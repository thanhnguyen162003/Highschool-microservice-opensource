namespace Domain.Models.Settings
{
    public class WorkerSetting
    {
        public int? MinWorkerCount { get; set; }
        public int? MaxWorkerCount { get; set; } 
        public int? ScaleInterval { get; set; }
        public int? TaskThresholdToScaleUp { get; set; }
        public int? TaskThresholdToScaleDown { get; set; } 
    }
}
