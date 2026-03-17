using System;
using System.IO;
using UnityEngine;

namespace MemoryGameTools
{
    /// <summary>
    /// WAV 文件保存工具类（纯静态，无 MonoBehaviour 依赖）
    /// 将 AudioClip 保存为标准 16-bit PCM WAV 文件
    /// </summary>
    public static class SavWav
    {
        private const int HeaderSize = 44;

        public static bool Save(string filepath, AudioClip clip)
        {
            if (clip == null) return false;

            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            using (var fs = CreateEmpty(filepath))
            {
                ConvertAndWrite(fs, samples);
                WriteHeader(fs, clip);
            }

            return true;
        }

        private static FileStream CreateEmpty(string filepath)
        {
            if (File.Exists(filepath)) File.Delete(filepath);
            var fs = new FileStream(filepath, FileMode.Create);
            for (int i = 0; i < HeaderSize; i++) fs.WriteByte(0);
            return fs;
        }

        private static void ConvertAndWrite(FileStream fs, float[] samples)
        {
            short[] intData = new short[samples.Length];
            byte[] bytesData = new byte[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * short.MaxValue);
                byte[] byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fs.Write(bytesData, 0, bytesData.Length);
        }

        private static void WriteHeader(FileStream fs, AudioClip clip)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            fs.Seek(0, SeekOrigin.Begin);

            byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fs.Write(riff, 0, 4);

            byte[] chunkSize = BitConverter.GetBytes(fs.Length - 8);
            fs.Write(chunkSize, 0, 4);

            byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fs.Write(wave, 0, 4);

            byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fs.Write(fmt, 0, 4);

            byte[] subChunk1 = BitConverter.GetBytes(16);
            fs.Write(subChunk1, 0, 4);

            byte[] audioFormat = BitConverter.GetBytes((ushort)1);
            fs.Write(audioFormat, 0, 2);

            byte[] numChannels = BitConverter.GetBytes((ushort)channels);
            fs.Write(numChannels, 0, 2);

            byte[] sampleRate = BitConverter.GetBytes(hz);
            fs.Write(sampleRate, 0, 4);

            byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
            fs.Write(byteRate, 0, 4);

            byte[] blockAlign = BitConverter.GetBytes((ushort)(channels * 2));
            fs.Write(blockAlign, 0, 2);

            byte[] bitsPerSample = BitConverter.GetBytes((ushort)16);
            fs.Write(bitsPerSample, 0, 2);

            byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fs.Write(dataString, 0, 4);

            byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            fs.Write(subChunk2, 0, 4);
        }
    }
}
