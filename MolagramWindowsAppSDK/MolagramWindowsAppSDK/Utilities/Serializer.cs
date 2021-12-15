using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MolagramWindowsAppSDK.Utilities
{
    public static class Serializer
    {
        internal static async Task<T> FromFile<T>(string fileName)
        {
            try
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(fileName);
                var stream = await file.OpenStreamForReadAsync();

                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(stream);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error
                return default(T);
            }
        }
    }
}
