//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents the collection of SelectedDates for the Calendar Control.
    /// </summary>
    public sealed class SelectedDatesCollection : ObservableCollection<DateTime>
    {
        #region Data
        private readonly Collection<DateTime> _addedItems;
        private readonly Collection<DateTime> _removedItems;
        private readonly Thread _dispatcherThread;
        private bool _isAddingRange;
        private readonly PersianCalendar _owner;
        private DateTime? _maximumDate;
        private DateTime? _minimumDate;

        #endregion Data

        /// <summary>
        /// Initializes a new instance of the CalendarSelectedDatesCollection class.
        /// </summary>
        /// <param name="owner"></param>
        public SelectedDatesCollection(PersianCalendar owner)
        {
            _dispatcherThread = Thread.CurrentThread;
            _owner = owner;
            _addedItems = new Collection<DateTime>();
            _removedItems = new Collection<DateTime>();
        }

        #region Internal Properties

        internal DateTime? MinimumDate
        {
            get
            {
                if (Count < 1)
                {
                    return null;
                }

                if (!_minimumDate.HasValue)
                {
                    DateTime result = this[0];
                    foreach (DateTime selectedDate in this)
                    {
                        if (DateTime.Compare(selectedDate, result) < 0)
                        {
                            result = selectedDate;
                        }
                    }

                    _maximumDate = result;
                }

                return _minimumDate;
            }
        }

        internal DateTime? MaximumDate
        {
            get
            {
                if (Count < 1)
                {
                    return null;
                }

                if (!_maximumDate.HasValue)
                {
                    DateTime result = this[0];
                    foreach (DateTime selectedDate in this)
                    {
                        if (DateTime.Compare(selectedDate, result) > 0)
                        {
                            result = selectedDate;
                        }
                    }

                    _maximumDate = result;
                }

                return _maximumDate;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a range of dates to the Calendar SelectedDates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void AddRange(DateTime start, DateTime end)
        {
            BeginAddRange();

            // If CalendarSelectionMode.SingleRange and a user programmatically tries to add multiple ranges, we will throw away the old range and replace it with the new one.
            if (_owner.SelectionMode == CalendarSelectionMode.SingleRange && Count > 0)
            {
                ClearInternal();
            }

            foreach (DateTime current in GetDaysInRange(start, end))
            {
                Add(current);
            }

            EndAddRange();
        }

        #endregion Public Methods

        #region Protected methods

        /// <summary>
        /// Clears all the items of the SelectedDates.
        /// </summary>
        protected override void ClearItems()
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
            }

            // Turn off highlight
            _owner.HoverStart = null;
            ClearInternal(true /*fireChangeNotification*/);
        }

        /// <summary>
        /// Inserts the item in the specified position of the SelectedDates collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, DateTime item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
            }

            if (!Contains(item))
            {
                Collection<DateTime> addedItems = new Collection<DateTime>();

                bool isCleared = CheckSelectionMode();

                if (PersianCalendar.IsValidDateSelection(_owner, item))
                {
                    // If the Collection is cleared since it is SingleRange and it had another range
                    // set the index to 0
                    if (isCleared)
                    {
                        index = 0;
                        isCleared = false;
                    }

                    base.InsertItem(index, item);
                    UpdateMinMax(item);

                    // The event fires after SelectedDate changes
                    if (index == 0 && !(_owner.SelectedDate.HasValue && DateTime.Compare(_owner.SelectedDate.Value, item) == 0))
                    {
                        _owner.SelectedDate = item;
                    }

                    if (!_isAddingRange)
                    {
                        addedItems.Add(item);

                        RaiseSelectionChanged(_removedItems, addedItems);
                        _removedItems.Clear();
                        int monthDifference = DateTimeHelper.CompareYearMonth(item, _owner.DisplayDateInternal);

                        if (monthDifference < 2 && monthDifference > -2)
                        {
                            _owner.UpdateCellItems();
                        }
                    }
                    else
                    {
                        _addedItems.Add(item);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue));
                }
            }
        }

        /// <summary>
        /// Removes the item at the specified position.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
            }

            if (index >= Count)
            {
                base.RemoveItem(index);
                ClearMinMax();
            }
            else
            {
                Collection<DateTime> addedItems = new Collection<DateTime>();
                Collection<DateTime> removedItems = new Collection<DateTime>();
                int monthDifference = DateTimeHelper.CompareYearMonth(this[index], _owner.DisplayDateInternal);

                removedItems.Add(this[index]);
                base.RemoveItem(index);
                ClearMinMax();

                // The event fires after SelectedDate changes
                if (index == 0)
                {
                    if (Count > 0)
                    {
                        _owner.SelectedDate = this[0];
                    }
                    else
                    {
                        _owner.SelectedDate = null;
                    }
                }

                RaiseSelectionChanged(removedItems, addedItems);

                if (monthDifference < 2 && monthDifference > -2)
                {
                    _owner.UpdateCellItems();
                }
            }
        }

        /// <summary>
        /// The object in the specified index is replaced with the provided item.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, DateTime item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
            }

            if (!Contains(item))
            {
                Collection<DateTime> addedItems = new Collection<DateTime>();
                Collection<DateTime> removedItems = new Collection<DateTime>();

                if (index >= Count)
                {
                    base.SetItem(index, item);
                    UpdateMinMax(item);
                }
                else
                {
                    if (item != null && DateTime.Compare(this[index], item) != 0 && PersianCalendar.IsValidDateSelection(_owner, item))
                    {
                        removedItems.Add(this[index]);
                        base.SetItem(index, item);
                        UpdateMinMax(item);

                        addedItems.Add(item);

                        // The event fires after SelectedDate changes
                        if (index == 0 && !(_owner.SelectedDate.HasValue && DateTime.Compare(_owner.SelectedDate.Value, item) == 0))
                        {
                            _owner.SelectedDate = item;
                        }

                        RaiseSelectionChanged(removedItems, addedItems);

                        int monthDifference = DateTimeHelper.CompareYearMonth(item, _owner.DisplayDateInternal);

                        if (monthDifference < 2 && monthDifference > -2)
                        {
                            _owner.UpdateCellItems();
                        }
                    }
                }
            }
        }

        #endregion Protected methods

        #region Internal Methods

        /// <summary>
        /// Adds a range of dates to the Calendar SelectedDates.
        /// </summary>
        /// <remarks>
        /// Helper version of AddRange for mouse drag selection. 
        /// This version guarantees no exceptions will be thrown by removing blackout days from the range before adding to the collection      
        /// </remarks>
        internal void AddRangeInternal(DateTime start, DateTime end)
        {
            BeginAddRange();

            // In Mouse Selection we allow the user to be able to add multiple ranges in one action in MultipleRange Mode
            // In SingleRange Mode, we only add the first selected range
            DateTime lastAddedDate = start;
            foreach (DateTime current in GetDaysInRange(start, end))
            {
                if (PersianCalendar.IsValidDateSelection(_owner, current))
                {
                    Add(current);
                    lastAddedDate = current;
                }
                else
                {
                    if (_owner.SelectionMode == CalendarSelectionMode.SingleRange)
                    {
                        _owner.CurrentDate = lastAddedDate;
                        break;
                    }
                }
            }

            EndAddRange();
        }

        internal void ClearInternal()
        {
            ClearInternal(false /*fireChangeNotification*/);
        }

        internal void ClearInternal(bool fireChangeNotification)
        {
            if (Count > 0)
            {
                foreach (DateTime item in this)
                {
                    _removedItems.Add(item);
                }

                base.ClearItems();
                ClearMinMax();

                if (fireChangeNotification)
                {
                    if (_owner.SelectedDate != null)
                    {
                        _owner.SelectedDate = null;
                    }

                    if (_removedItems.Count > 0)
                    {
                        Collection<DateTime> addedItems = new Collection<DateTime>();
                        RaiseSelectionChanged(_removedItems, addedItems);
                        _removedItems.Clear();
                    }

                    _owner.UpdateCellItems();
                }
            }
        }


        internal void Toggle(DateTime date)
        {
            if (PersianCalendar.IsValidDateSelection(_owner, date))
            {
                switch (_owner.SelectionMode)
                {
                    case CalendarSelectionMode.SingleDate:
                        {
                            if (!_owner.SelectedDate.HasValue || DateTimeHelper.CompareDays(_owner.SelectedDate.Value, date) != 0)
                            {
                                _owner.SelectedDate = date;
                            }
                            else
                            {
                                _owner.SelectedDate = null;
                            }

                            break;
                        }

                    case CalendarSelectionMode.MultipleRange:
                        {
                            if (!Remove(date))
                            {
                                Add(date);
                            }

                            break;
                        }

                    default:
                        {
                            Debug.Assert(false);
                            break;
                        }
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void RaiseSelectionChanged(IList removedItems, IList addedItems)
        {
            _owner.OnSelectedDatesCollectionChanged(new CalendarSelectionChangedEventArgs(PersianCalendar.SelectedDatesChangedEvent, removedItems, addedItems));
        }

        private void BeginAddRange()
        {
            Debug.Assert(!_isAddingRange);
            _isAddingRange = true;
        }

        private void EndAddRange()
        {
            Debug.Assert(_isAddingRange);

            _isAddingRange = false;
            RaiseSelectionChanged(_removedItems, _addedItems);
            _removedItems.Clear();
            _addedItems.Clear();
            _owner.UpdateCellItems();
        }

        private bool CheckSelectionMode()
        {
            if (_owner.SelectionMode == CalendarSelectionMode.None)
            {
                throw new InvalidOperationException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidOperation));
            }

            if (_owner.SelectionMode == CalendarSelectionMode.SingleDate && Count > 0)
            {
                throw new InvalidOperationException(SR.Get(SRID.Calendar_CheckSelectionMode_InvalidOperation));
            }

            // if user tries to add an item into the SelectedDates in SingleRange mode, we throw away the old range and replace it with the new one
            // in order to provide the removed items without an additional event, we are calling ClearInternal
            if (_owner.SelectionMode == CalendarSelectionMode.SingleRange && !_isAddingRange && Count > 0)
            {
                ClearInternal();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsValidThread()
        {
            return Thread.CurrentThread == _dispatcherThread;
        }

        private void UpdateMinMax(DateTime date)
        {
            if ((!_maximumDate.HasValue) || (date > _maximumDate.Value))
            {
                _maximumDate = date;
            }

            if ((!_minimumDate.HasValue) || (date < _minimumDate.Value))
            {
                _minimumDate = date;
            }
        }

        private void ClearMinMax()
        {
            _maximumDate = null;
            _minimumDate = null;
        }

        private static IEnumerable<DateTime> GetDaysInRange(DateTime start, DateTime end)
        {
            // increment parameter specifies if the Days were selected in Descending order or Ascending order
            // based on this value, we add the days in the range either in Ascending order or in Descending order
            int increment = GetDirection(start, end);

            DateTime? rangeStart = start;

            do
            {
                yield return rangeStart.Value;
                rangeStart = DateTimeHelper.AddDays(rangeStart.Value, increment);
            }
            while (rangeStart.HasValue && DateTime.Compare(end, rangeStart.Value) != -increment);
        }

        private static int GetDirection(DateTime start, DateTime end)
        {
            return (DateTime.Compare(end, start) >= 0) ? 1 : -1;
        }

        #endregion Private Methods
    }
}