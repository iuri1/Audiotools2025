AudioTools2025 — Real-Time Microphone Spectrum for Grasshopper

AudioTools2025 is a lightweight Grasshopper plugin that captures **real-time microphone audio** and computes a **frequency spectrum (FFT)** directly inside Grasshopper — **without Firefly**.

It includes all required audio libraries (NAudio) and works out of the box.

---

 Features

✔ Real-time microphone capture (NAudio)  
✔ Fast FFT (Cooley–Tukey)  
✔ Automatic refresh every frame  
✔ Configurable frequency bins  
✔ Microphone device selector  
✔ No Firefly required  
✔ Lightweight and open-source  

---

Components

### **Mic Spectrum**
The main component of the plugin.

**Inputs:**
- **On (bool)** — enables microphone capture  
- **Bins (int)** — number of FFT frequency bands (optional)  
- **Device (int)** — microphone device index (optional)

**Outputs:**
- **Magnitudes (list<double>)** — FFT magnitude per band  
- **Frequencies (list<double>)** — center frequency of each band (Hz)

---

Installation (Important!)

AudioTools2025 requires **NAudio**, a .NET audio library.  
The required DLLs are already included in the plugin download.

### 🟦 1. Download the release
From the **Releases** page, download:

- `AudioTools2025.gha`
- `NAudio.dll`
- `NAudio.Asio.dll` *(optional — only for ASIO devices)*
- `NAudio.WinMM.dll`

🟦 2. Place all files in the same folder:
%APPDATA%\Grasshopper\Libraries

3. If Windows blocks the file, right-click → **Properties** → *Unblock*.  
4. Restart Rhino + Grasshopper.

You will find the plugin under the tab:

AudioTools2025 → Audio → Mic Spectrum

## 🛠 Requirements

- **Rhino 6, 7 or 8**
- **Grasshopper**
- **Windows OS**
- **.NET Framework 4.8**
- **NAudio (automatically included)**

---

## 📡 How It Works

AudioTools2025 uses:

- **WaveInEvent** (NAudio) for microphone capture  
- **44100 Hz mono stream** for stability  
- Real-time buffering with thread-safe locks  
- A lightweight FFT to compute magnitudes and frequencies

The component automatically expires its solution every frame to keep the audio reactive.

---

## 🖥 Example

Connect the outputs to graphs, meshes, color fields, or geometry parameters to create audio-responsive systems:
\\
Mic Spectrum → Graph Mapper → Extrude
Mic Spectrum → Color Gradient → Custom Preview
Mic Spectrum → Mesh Displacement

Experimental uses:

LED wall simulation

Parametric sculpture reacting to music

Microphone-controlled façade shading

Live visualization for performances


AudioTools2025/
 ├── MicSpectrumComponent.cs
 ├── AudioTools2025Info.cs
 ├── Properties/
 │    └── Resources.resx (icon embedded here)
 ├── res/
 │    └── mic_icon_24x24.png
 └── packages.config



 Contributing

Pull requests are welcome!

If you want to add:

More audio analysis tools

Beat detection

Spectrograms

Audio playback nodes

Filters (low-pass, high-pass, etc.)
I’ll be happy to help expand the project.


License

MIT License — Free for personal and commercial use.


Credits

Developed by Iuri Trombini
Built with:

C#

Grasshopper SDK

NAudio

Support the Project

If this plugin helps your workflow, consider giving it a star on GitHub ⭐
It motivates future updates!
