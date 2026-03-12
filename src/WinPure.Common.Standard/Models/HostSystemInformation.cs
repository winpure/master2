using System.Collections.Generic;

namespace WinPure.Common.Models
{
    internal class HostSystemInformation
    {
        public string Windows { get; set; }
        public string AppBitness { get; set; }
        public string SystemBitness { get; set; }
        public string Processor { get; set; }
        public string ProcessorDescription { get; set; }
        public int ProcessorCount { get; set; }
        public int CoresCount { get; set; }
        public int ProcessorThreadCount { get; set; }
        public double MemorySize { get; set; }
        public double FreeMemorySize { get; set; }
        public List<DriveInfo> Drives { get; set; }
        public override string ToString()
        {
            return $"WINDOWS: {Windows} {SystemBitness}| APPLICATION BIT: {AppBitness}| PROCESSOR:{Processor} COUNT: {ProcessorCount} CORES: {CoresCount} THREADS: {ProcessorThreadCount}| MEMORY: {MemorySize}";
        }
    }
}