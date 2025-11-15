using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Grasshopper.Kernel;
using NAudio.Wave;

namespace AudioTools2025
{
    public class MicSpectrumComponent : GH_Component
    {
        private static WaveInEvent _waveIn;
        private static float[] _samples = new float[0];
        private static readonly object _lock = new object();
        private static int _currentDevice = -1;

        public MicSpectrumComponent()
            : base(
                  "Mic Spectrum",
                  "MicSpec",
                  "Real-time microphone spectrum using FFT.",
                  "AudioTools2025",
                  "Audio"
                  )
        { }

        public override Guid ComponentGuid =>
            new Guid("8F4C0C2E-9B2A-4E17-9F33-5D5A2F0D3B11");

        // ---------------------------
        //  ICON LOADER
        // ---------------------------
        protected override Bitmap Icon =>
            LoadIcon("AudioTools2025.res.mic_icon_24x24.png");

        private Bitmap LoadIcon(string resourcePath)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using (var s = asm.GetManifestResourceStream(resourcePath))
            {
                if (s != null) return new Bitmap(s);
            }
            return null;
        }

        // ---------------------------
        // INPUTS
        // ---------------------------
        protected override void RegisterInputParams(GH_InputParamManager p)
        {
            p.AddBooleanParameter("On", "On", "Enable microphone input.", GH_ParamAccess.item);
            p.AddIntegerParameter("Bins", "N", "Number of frequency bands.", GH_ParamAccess.item);
            p[1].Optional = true;

            p.AddIntegerParameter("Device", "D", "Microphone device index.", GH_ParamAccess.item);
            p[2].Optional = true;
        }

        // ---------------------------
        // OUTPUTS
        // ---------------------------
        protected override void RegisterOutputParams(GH_OutputParamManager p)
        {
            p.AddNumberParameter("Magnitudes", "M", "FFT magnitude bands.", GH_ParamAccess.list);
            p.AddNumberParameter("Frequencies", "F", "Frequency (Hz) of each band.", GH_ParamAccess.list);
        }

        // ---------------------------
        // MAIN SOLVER
        // ---------------------------
        protected override void SolveInstance(IGH_DataAccess da)
        {
            bool on = false;
            int bins = 64;
            int device = 0;

            da.GetData(0, ref on);
            da.GetData(1, ref bins);
            da.GetData(2, ref device);

            if (!on)
            {
                StopMic();
                da.SetDataList(0, new List<double>());
                da.SetDataList(1, new List<double>());
                return;
            }

            StartMic(device);

            float[] buf;
            lock (_lock)
            {
                buf = (float[])_samples.Clone();
            }

            if (buf.Length == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "Waiting for microphone audio...");
                this.ExpireSolution(true);
                return;
            }

            List<double> samples = new List<double>(buf.Length);
            foreach (float f in buf) samples.Add(f);

            ComputeSpectrum(samples, 44100, bins, out var mags, out var freqs);

            da.SetDataList(0, mags);
            da.SetDataList(1, freqs);

            this.ExpireSolution(true);
        }

        // ---------------------------
        // MICROPHONE HANDLING
        // ---------------------------
        private void StartMic(int device)
        {
            if (device < 0 || device >= WaveIn.DeviceCount) device = 0;

            if (_waveIn != null && _currentDevice == device)
                return;

            StopMic();

            try
            {
                _waveIn = new WaveInEvent();
                _waveIn.DeviceNumber = device;
                _waveIn.WaveFormat = new WaveFormat(44100, 1);
                _waveIn.BufferMilliseconds = 50;
                _waveIn.DataAvailable += OnDataAvailable;

                _currentDevice = device;
                _waveIn.StartRecording();

                // List devices in GH remark
                string msg = "Audio devices:";
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    var caps = WaveIn.GetCapabilities(i);
                    msg += $"\n {i}: {caps.ProductName}";
                }
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, msg);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Error starting microphone: " + ex.Message);
                StopMic();
            }
        }

        private void StopMic()
        {
            if (_waveIn == null) return;

            try
            {
                _waveIn.DataAvailable -= OnDataAvailable;
                _waveIn.StopRecording();
                _waveIn.Dispose();
            }
            catch { }

            _waveIn = null;
            _currentDevice = -1;

            lock (_lock)
            {
                _samples = new float[0];
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            int count = e.BytesRecorded / 2;
            float[] buff = new float[count];

            for (int i = 0; i < count; i++)
            {
                short s = BitConverter.ToInt16(e.Buffer, i * 2);
                buff[i] = s / 32768f;
            }

            lock (_lock)
            {
                _samples = buff;
            }
        }

        // ---------------------------
        // FFT
        // ---------------------------
        private static void ComputeSpectrum(
            List<double> samples, double sr, int bins,
            out List<double> mags, out List<double> freqs)
        {
            int n = NextPow2(samples.Count);
            while (samples.Count < n)
                samples.Add(0.0);

            Complex[] buf = new Complex[n];
            for (int i = 0; i < n; i++)
                buf[i] = new Complex(samples[i], 0);

            FFT(buf);

            int half = n / 2;

            mags = new List<double>(bins);
            freqs = new List<double>(bins);

            int step = half / bins;

            for (int b = 0; b < bins; b++)
            {
                int start = b * step;
                int end = Math.Min(half, (b + 1) * step);

                double sum = 0;
                for (int i = start; i < end; i++)
                    sum += buf[i].Magnitude;

                mags.Add(sum / (end - start));

                double f0 = start * sr / n;
                double f1 = end * sr / n;
                freqs.Add((f0 + f1) * 0.5);
            }
        }

        private static int NextPow2(int v)
        {
            int p = 1;
            while (p < v) p <<= 1;
            return p;
        }

        private static void FFT(Complex[] a)
        {
            int n = a.Length;
            int j = 0;

            for (int i = 1; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0)
                {
                    j ^= bit;
                    bit >>= 1;
                }
                j ^= bit;

                if (i < j)
                {
                    var t = a[i];
                    a[i] = a[j];
                    a[j] = t;
                }
            }

            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = -2 * Math.PI / len;
                Complex wlen = new Complex(Math.Cos(ang), Math.Sin(ang));

                for (int i = 0; i < n; i += len)
                {
                    Complex w = Complex.One;
                    int half = len >> 1;

                    for (int k = 0; k < half; k++)
                    {
                        Complex u = a[i + k];
                        Complex v = a[i + k + half] * w;

                        a[i + k] = u + v;
                        a[i + k + half] = u - v;

                        w *= wlen;
                    }
                }
            }
        }

        public override void RemovedFromDocument(GH_Document doc)
        {
            base.RemovedFromDocument(doc);
            StopMic();
        }
    }
}
