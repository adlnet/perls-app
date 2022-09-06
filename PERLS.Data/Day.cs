using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace PERLS.Data
{
    /// <summary>
    /// The day of week.
    /// </summary>
    [Serializable]
    public class Day : INotifyPropertyChanged
    {
        bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Day"/> class.
        /// </summary>
        /// <param name="day">The day to represent.</param>
        public Day(DayOfWeek day)
        {
            StoredDay = day;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the background color for the days of week. NOTE! This is currently used as a workaround for
        /// issue 9511 in xamarin forms. Collection views do not apply the selected visual state to pre selected
        /// items, requiring us to manually change the background color.
        /// </summary>
        /// <value>
        /// The background color for the days of week.
        /// </value>
        public Color CellBackgroundColor => IsSelected ? (Color)Application.Current.Resources["SecondaryColor"] : Color.White;

        /// <summary>
        /// Gets or sets a value indicating whether the day of week is selected.
        /// </summary>
        /// <value>
        /// The value indicating whether the day of week is selected.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
                NotifyPropertyChanged(nameof(CellBackgroundColor));
            }
        }

        /// <summary>
        /// Gets the stored enum.
        /// </summary>
        /// <value>
        /// The name of the day.
        /// </value>
        public DayOfWeek StoredDay { get; }

        /// <summary>
        /// Gets the short string name of the day.
        /// </summary>
        /// <value>
        /// The short string name of the day.
        /// </value>
        public string ShortStringDay
        {
            get
            {
                return ToShortString();
            }
        }

        /// <summary>
        /// Accesses the correct short string for the collection of labels.
        /// </summary>
        /// <returns>The string for the collection of labels.</returns>
        public string ToShortString()
        {
            return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(StoredDay);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
