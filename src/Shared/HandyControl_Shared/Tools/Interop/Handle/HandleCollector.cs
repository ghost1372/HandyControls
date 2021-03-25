// reference from https://referencesource.microsoft.com/#WindowsBase/Shared/MS/Win32/HandleCollector.cs,d0f99220d8e1b708

using System;
using System.Runtime.InteropServices;

namespace HandyControl.Tools.Interop
{
    public static class HandleCollector
    {
        private static HandleType[] HandleTypes;
        private static int HandleTypeCount;

        private static readonly object HandleMutex = new object();

        public static IntPtr Add(IntPtr handle, int type)
        {
            HandleTypes[type - 1].Add();
            return handle;
        }

        [System.Security.SecuritySafeCritical]
        public static SafeHandle Add(SafeHandle handle, int type)
        {
            HandleTypes[type - 1].Add();
            return handle;
        }

        public static void Add(int type)
        {
            HandleTypes[type - 1].Add();
        }

        public static int RegisterType(string typeName, int expense, int initialThreshold)
        {
            lock (HandleMutex)
            {
                if (HandleTypeCount == 0 || HandleTypeCount == HandleTypes.Length)
                {
                    HandleType[] newTypes = new HandleType[HandleTypeCount + 10];
                    if (HandleTypes != null)
                    {
                        Array.Copy(HandleTypes, 0, newTypes, 0, HandleTypeCount);
                    }
                    HandleTypes = newTypes;
                }

                HandleTypes[HandleTypeCount++] = new HandleType(expense, initialThreshold);
                return HandleTypeCount;
            }
        }

        public static IntPtr Remove(IntPtr handle, int type)
        {
            HandleTypes[type - 1].Remove();
            return handle;
        }

        [System.Security.SecuritySafeCritical]
        public static SafeHandle Remove(SafeHandle handle, int type)
        {
            HandleTypes[type - 1].Remove();
            return handle;
        }

        public static void Remove(int type)
        {
            HandleTypes[type - 1].Remove();
        }

        public class HandleType
        {
            private readonly int _initialThreshHold;
            private int _threshHold;
            private int _handleCount;
            private readonly int _deltaPercent;

            public HandleType(int expense, int initialThreshHold)
            {
                _initialThreshHold = initialThreshHold;
                _threshHold = initialThreshHold;
                _deltaPercent = 100 - expense;
            }

            public void Add()
            {
                lock (this)
                {
                    _handleCount++;
                    var performCollect = NeedCollection();

                    if (!performCollect)
                    {
                        return;
                    }
                }

                GC.Collect();

                var sleep = (100 - _deltaPercent) / 4;
                System.Threading.Thread.Sleep(sleep);
            }

            public bool NeedCollection()
            {

                if (_handleCount > _threshHold)
                {
                    _threshHold = _handleCount + _handleCount * _deltaPercent / 100;
                    return true;
                }

                var oldThreshHold = 100 * _threshHold / (100 + _deltaPercent);
                if (oldThreshHold >= _initialThreshHold && _handleCount < (int) (oldThreshHold * .9F))
                {
                    _threshHold = oldThreshHold;
                }

                return false;
            }

            public void Remove()
            {
                lock (this)
                {
                    _handleCount--;

                    _handleCount = Math.Max(0, _handleCount);
                }
            }
        }
    }
}
