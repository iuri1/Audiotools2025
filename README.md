# 🎤 AudioTools2025 — Real-Time Microphone Spectrum for Grasshopper

AudioTools2025 is a lightweight Grasshopper plugin that captures **real-time microphone audio** and computes a **frequency spectrum (FFT)** directly inside Grasshopper — **without Firefly**.

It includes all required audio libraries (NAudio) and works out of the box.

---

## 🚀 Features

✔ Real-time microphone capture (NAudio)  
✔ Fast FFT (Cooley–Tukey)  
✔ Automatic refresh every frame  
✔ Configurable frequency bins  
✔ Microphone device selector  
✔ No Firefly required  
✔ Lightweight and open-source  

---

## 🧩 Components

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

# 📦 Installation (Important!)

AudioTools2025 requires **NAudio**, a .NET audio library.  
The required DLLs are already included in the plugin download.

### 🟦 1. Download the release
From the **Releases** page, download:

- `AudioTools2025.gha`
- `NAudio.dll`
- `NAudio.Asio.dll` *(optional — only for ASIO devices)*
- `NAudio.WinMM.dll`

### 🟦 2. Place all files in the same folder:
