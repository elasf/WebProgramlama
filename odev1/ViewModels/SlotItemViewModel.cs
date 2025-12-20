using System;

namespace odev1.ViewModels
{
    public class SlotItemViewModel
    {
        
        public TimeSpan StartTime { get; set; }

   
        public string TimeLabel => StartTime.ToString(@"hh\:mm");


        public bool IsAvailable { get; set; }
    }
}
