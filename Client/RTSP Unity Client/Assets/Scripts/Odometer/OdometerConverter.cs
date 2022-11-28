using System;
using System.Collections.Generic;

namespace RtspTest.Domains.Odometer
{
    public static class OdometerUpdater
    {
        private static Dictionary<string, Action<OdometerOperationResult, OdometerData>> UpdateMethods =
            new()
            {
                ["default"] = (from, to) =>
                {
                    to.UpdateValues(false, 0f);
                }
            };

        public static void UpdateValue(OdometerOperationResult dataFrom, OdometerData dataTo)
        {
            if (UpdateMethods.ContainsKey(dataFrom.operation))
            {
                UpdateMethods[dataFrom.operation](dataFrom, dataTo);
                return;
            }

            UpdateMethods["default"](dataFrom, dataTo);
        }

        public static void AddUpdateMethods(string key, Action<OdometerOperationResult, OdometerData> method)
        {
            UpdateMethods.Add(key, method);
        }

        public static void RemoveUpdateMethods(string key)
        {
            UpdateMethods.Remove(key);
        }

    }
}
