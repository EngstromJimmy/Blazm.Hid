using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazm.Hid
{
    public class HidNavigator : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public HidNavigator(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/Blazm.Hid/Blazm.Hid.js").AsTask());
        }

        public async Task<HidDevice> RequestDeviceAsync(HidDeviceRequestOptions options)
        {
            string json = JsonConvert.SerializeObject(options,
            Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var module = await moduleTask.Value;
            var device = await module.InvokeAsync<HidDevice>("requestDevice", json);
            device.InitHidDevice(this);
            return device;
        }


        public async Task<List<HidDevice>> GetDevicesAsync()
        {
            var module = await moduleTask.Value;
            var devices = await module.InvokeAsync<HidDevice[]>("getDevices");
            //Init all the devices
            foreach (var d in devices)
            {
                d.InitHidDevice(this);
            }
            return devices.ToList();
        }

        private List<DotNetObjectReference<HidDevice>> NotificationHandlers = new();

        public async Task<bool> OpenDeviceAsync(HidDevice device)
        {
            var module = await moduleTask.Value;
            var handler = DotNetObjectReference.Create(device);
            NotificationHandlers.Add(handler);
            return await module.InvokeAsync<bool>("openDevice", device.Id,handler );
        }

        public async Task<bool> CloseDeviceAsync(HidDevice device)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("closeDevice", device.Id);
        }

        public async Task SendReportAsync(HidDevice device, int reportId, byte[] data)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("sendReport", device.Id, reportId, data);
        }

        public async Task SendFeatureReportAsync(HidDevice device, int reportId, byte[] data)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("sendFeatureReport", device.Id, data);
        }


        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
