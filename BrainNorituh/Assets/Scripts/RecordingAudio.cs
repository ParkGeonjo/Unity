using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Text;
using TMPro;

public class RecordingAudio : MonoBehaviour
{
	// Force save as 16-bit .wav
	const int BlockSize_16Bit = 2;

	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingLengthSec = 15;
	private int _recordingHZ = 22050;

	// 녹음 여부, 녹음 결과
	public bool isRecord = false;
	public string recordText;

	// 녹음 시작, 종료 이벤트
	public event EventHandler OnRecordFail;
	public event EventHandler OnRecordEnd;

	private void Start()
	{
		_microphoneID = Microphone.devices[0];
	}

	// 음성인식 시작, 종료
	public void Recording()
	{
		if (!isRecord)
		{
			isRecord = true;
			Debug.Log("Start recording");
			_recording = Microphone.Start(_microphoneID, false, _recordingLengthSec, _recordingHZ);
		}
        else
		{
			isRecord = false;
			if (Microphone.IsRecording(_microphoneID))
			{
				Microphone.End(_microphoneID);

				Debug.Log("Stop recording");
				if (_recording == null)
				{
					Debug.LogError("nothing recorded");
					return;
				}
				byte[] byteData = getByteFromAudioClip(_recording);

				StartCoroutine(PostVoice(url, byteData));
			}
            else
            {
				if (OnRecordFail != null)
				{
					OnRecordFail(this, EventArgs.Empty);
				}
			}
			return;
		}
	}

	public void StopRecording()
	{
		isRecord = false;
		if (Microphone.IsRecording(_microphoneID))
		{
			Microphone.End(_microphoneID);

			Debug.Log("Stop recording");
			if (_recording == null)
			{
				Debug.LogError("nothing recorded");
				return;
			}
			// audio clip to byte array
			byte[] byteData = getByteFromAudioClip(_recording);

			// 녹음된 audioclip api 서버로 보냄
			StartCoroutine(PostVoice(url, byteData));
		}
		else
		{
			if (OnRecordFail != null)
			{
				OnRecordFail(this, EventArgs.Empty);
			}
		}
		return;
	}

	public void  ExitRecording()
	{
		isRecord = false;
		if (Microphone.IsRecording(_microphoneID))
		{
			Microphone.End(_microphoneID);

			Debug.Log("stop recording");
		}
		return;
	}

	private byte[] getByteFromAudioClip(AudioClip audioClip)
	{
		MemoryStream stream = new MemoryStream();
		const int headerSize = 44;
		ushort bitDepth = 16;

		int fileSize = audioClip.samples * BlockSize_16Bit + headerSize;

		// audio clip의 정보들을 file stream에 추가(링크 참고 함수 선언)
		WriteFileHeader(ref stream, fileSize);
		WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
		WriteFileData(ref stream, audioClip, bitDepth);

		// stream을 array형태로 바꿈
		byte[] bytes = stream.ToArray();

		return bytes;
	}

	// 받아온 값에 간편하게 접근하기 위한 JSON 선언
	[Serializable]
	public class VoiceRecognize
	{
		public string text;
	}

	string url = "url";

	private IEnumerator PostVoice(string url, byte[] data)
	{
		// request 생성
		WWWForm form = new WWWForm();
		UnityWebRequest request = UnityWebRequest.Post(url, form);

		// 요청 헤더 설정
		request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", "API-KEY-ID");
		request.SetRequestHeader("X-NCP-APIGW-API-KEY", "API-KEY");
		request.SetRequestHeader("Content-Type", "application/octet-stream");

		// 바디에 처리과정을 거친 Audio Clip data를 실어줌
		request.uploadHandler = new UploadHandlerRaw(data);

		// 요청을 보낸 후 response를 받을 때까지 대기
		yield return request.SendWebRequest();

		// 만약 response가 비어있다면 error
		if (request == null)
		{
			Debug.LogError(request.error);
		}
		else
		{
			// json 형태로 받음 {"text":"인식결과"}
			string message = request.downloadHandler.text;
			VoiceRecognize voiceRecognize = JsonUtility.FromJson<VoiceRecognize>(message);

			// Voice Server responded: 인식결과 저장.
			recordText = voiceRecognize.text;
			Debug.Log("Voice Server responded: " + voiceRecognize.text);

			if (OnRecordEnd != null)
			{
				OnRecordEnd(this, EventArgs.Empty);
				Debug.Log("음성인식 완료 이벤트 호출");
			}
		}
	}

	public static AudioClip ToAudioClip(string filePath)
	{
		if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith(Application.dataPath))
		{
			Debug.LogWarning("This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
			return null;
		}
		byte[] fileBytes = File.ReadAllBytes(filePath);
		return ToAudioClip(fileBytes, 0);
	}

	public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
	{
		//string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
		//string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
		int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
		UInt16 audioFormat = BitConverter.ToUInt16(fileBytes, 20);

		// NB: Only uncompressed PCM wav files are supported.
		string formatCode = FormatCode(audioFormat);
		Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

		UInt16 channels = BitConverter.ToUInt16(fileBytes, 22);
		int sampleRate = BitConverter.ToInt32(fileBytes, 24);
		//int byteRate = BitConverter.ToInt32 (fileBytes, 28);
		//UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
		UInt16 bitDepth = BitConverter.ToUInt16(fileBytes, 34);

		int headerOffset = 16 + 4 + subchunk1 + 4;
		int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);
		//Debug.LogFormat ("riff={0} wave={1} subchunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subchunk2={10} filesize={11}", riff, wave, subchunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subchunk2, fileBytes.Length);

		float[] data;
		switch (bitDepth)
		{
			case 8:
				data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 16:
				data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 24:
				data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 32:
				data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			default:
				throw new Exception(bitDepth + " bit depth is not supported.");
		}

		AudioClip audioClip = AudioClip.Create(name, data.Length, (int)channels, sampleRate, false);
		audioClip.SetData(data, 0);
		return audioClip;
	}

	#region wav file bytes to Unity AudioClip conversion methods

	private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		float[] data = new float[wavSize];

		sbyte maxValue = sbyte.MaxValue;

		int i = 0;
		while (i < wavSize)
		{
			data[i] = (float)source[i] / maxValue;
			++i;
		}

		return data;
	}

	private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = sizeof(Int16); // block size = 2
		int convertedSize = wavSize / x;

		float[] data = new float[convertedSize];

		Int16 maxValue = Int16.MaxValue;

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = 3; // block size = 3
		int convertedSize = wavSize / x;

		int maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			Buffer.BlockCopy(source, offset, block, 1, x);
			data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = sizeof(float); //  block size = 4
		int convertedSize = wavSize / x;

		Int32 maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	#endregion

	public static byte[] FromAudioClip(AudioClip audioClip)
	{
		string file;
		return FromAudioClip(audioClip, out file, false);
	}

	public static byte[] FromAudioClip(AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
	{
		MemoryStream stream = new MemoryStream();

		const int headerSize = 44;

		// get bit depth
		UInt16 bitDepth = 16; //BitDepth (audioClip);

		// NB: Only supports 16 bit
		//Debug.AssertFormat (bitDepth == 16, "Only converting 16 bit is currently supported. The audio clip data is {0} bit.", bitDepth);

		// total file size = 44 bytes for header format and audioClip.samples * factor due to float to Int16 / sbyte conversion
		int fileSize = audioClip.samples * BlockSize_16Bit + headerSize; // BlockSize (bitDepth)

		// chunk descriptor (riff)
		WriteFileHeader(ref stream, fileSize);
		// file header (fmt)
		WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
		// data chunks (data)
		WriteFileData(ref stream, audioClip, bitDepth);

		byte[] bytes = stream.ToArray();

		// Validate total bytes
		Debug.AssertFormat(bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count: {0} == {1}", bytes.Length, fileSize);

		// Save file to persistant storage location
		if (saveAsFile)
		{
			filepath = string.Format("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff"), "wav");
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));
			File.WriteAllBytes(filepath, bytes);
			//Debug.Log ("Auto-saved .wav file: " + filepath);
		}
		else
		{
			filepath = null;
		}

		stream.Dispose();

		return bytes;
	}

	#region write .wav file functions

	private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
	{
		int count = 0;
		int total = 12;

		// riff chunk id
		byte[] riff = Encoding.ASCII.GetBytes("RIFF");
		count += WriteBytesToMemoryStream(ref stream, riff, "ID");

		// riff chunk size
		int chunkSize = fileSize - 8; // total size - 8 for the other two fields in the header
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(chunkSize), "CHUNK_SIZE");

		byte[] wave = Encoding.ASCII.GetBytes("WAVE");
		count += WriteBytesToMemoryStream(ref stream, wave, "FORMAT");

		// Validate header
		Debug.AssertFormat(count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

		return count;
	}

	private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, UInt16 bitDepth)
	{
		int count = 0;
		int total = 24;

		byte[] id = Encoding.ASCII.GetBytes("fmt ");
		count += WriteBytesToMemoryStream(ref stream, id, "FMT_ID");

		int subchunk1Size = 16; // 24 - 8
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk1Size), "SUBCHUNK_SIZE");

		UInt16 audioFormat = 1;
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(audioFormat), "AUDIO_FORMAT");

		UInt16 numChannels = Convert.ToUInt16(channels);
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(numChannels), "CHANNELS");

		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");

		int byteRate = sampleRate * channels * BytesPerSample(bitDepth);
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(byteRate), "BYTE_RATE");

		UInt16 blockAlign = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(blockAlign), "BLOCK_ALIGN");

		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");

		// Validate format
		Debug.AssertFormat(count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

		return count;
	}

	private static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, UInt16 bitDepth)
	{
		int count = 0;
		int total = 8;

		// Copy float[] data from AudioClip
		float[] data = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(data, 0);

		byte[] bytes = ConvertAudioClipDataToInt16ByteArray(data);

		byte[] id = Encoding.ASCII.GetBytes("data");
		count += WriteBytesToMemoryStream(ref stream, id, "DATA_ID");

		int subchunk2Size = Convert.ToInt32(audioClip.samples * BlockSize_16Bit); // BlockSize (bitDepth)
		count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk2Size), "SAMPLES");

		// Validate header
		Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

		// Write bytes to stream
		count += WriteBytesToMemoryStream(ref stream, bytes, "DATA");

		// Validate audio data
		Debug.AssertFormat(bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);

		return count;
	}

	private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
	{
		MemoryStream dataStream = new MemoryStream();

		int x = sizeof(Int16);

		Int16 maxValue = Int16.MaxValue;

		int i = 0;
		while (i < data.Length)
		{
			dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
			++i;
		}
		byte[] bytes = dataStream.ToArray();

		// Validate converted bytes
		Debug.AssertFormat(data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

		dataStream.Dispose();

		return bytes;
	}

	private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes, string tag = "")
	{
		int count = bytes.Length;
		stream.Write(bytes, 0, count);
		//Debug.LogFormat ("WAV:{0} wrote {1} bytes.", tag, count);
		return count;
	}

	#endregion

	/// <summary>
	/// Calculates the bit depth of an AudioClip
	/// </summary>
	/// <returns>The bit depth. Should be 8 or 16 or 32 bit.</returns>
	/// <param name="audioClip">Audio clip.</param>
	public static UInt16 BitDepth(AudioClip audioClip)
	{
		UInt16 bitDepth = Convert.ToUInt16(audioClip.samples * audioClip.channels * audioClip.length / audioClip.frequency);
		Debug.AssertFormat(bitDepth == 8 || bitDepth == 16 || bitDepth == 32, "Unexpected AudioClip bit depth: {0}. Expected 8 or 16 or 32 bit.", bitDepth);
		return bitDepth;
	}

	private static int BytesPerSample(UInt16 bitDepth)
	{
		return bitDepth / 8;
	}

	private static int BlockSize(UInt16 bitDepth)
	{
		switch (bitDepth)
		{
			case 32:
				return sizeof(Int32); // 32-bit -> 4 bytes (Int32)
			case 16:
				return sizeof(Int16); // 16-bit -> 2 bytes (Int16)
			case 8:
				return sizeof(sbyte); // 8-bit -> 1 byte (sbyte)
			default:
				throw new Exception(bitDepth + " bit depth is not supported.");
		}
	}

	private static string FormatCode(UInt16 code)
	{
		switch (code)
		{
			case 1:
				return "PCM";
			case 2:
				return "ADPCM";
			case 3:
				return "IEEE";
			case 7:
				return "μ-law";
			case 65534:
				return "WaveFormatExtensable";
			default:
				Debug.LogWarning("Unknown wav code format:" + code);
				return "";
		}
	}
}
