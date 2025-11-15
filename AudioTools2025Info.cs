using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace AudioTools2025
{
    public class AudioTools2025Info : GH_AssemblyInfo
    {
        public override string Name => "AudioTools2025";

        public override Bitmap Icon => null; // ícone do plugin (não do componente)

        public override string Description =>
            "Audio tools for Grasshopper – microphone spectrum component.";

        public override Guid Id =>
            new Guid("0F4C21C1-9322-4D7B-9A56-23C1F3C7BB77"); // pode manter esse GUID

        public override string AuthorName => "Iuri Trombini";

        public override string AuthorContact => "@i.trombini";
    }
}