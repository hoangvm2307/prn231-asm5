
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace childTracking.Blazor.Client.Services
{
    public class DialogService : IDialogService
    {
        private readonly IJSRuntime _jsRuntime;

        public DialogService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> ShowConfirmation(string title, string message)
        {
            return await _jsRuntime.InvokeAsync<bool>("confirm", message);
        }

        public async Task ShowAlert(string title, string message)
        {
            await _jsRuntime.InvokeVoidAsync("alert", message);
        }

        public async Task ShowCustomDialog(string title, MarkupString content)
        {
            // Thực hiện hiển thị một dialog tùy chỉnh
            // Nếu sử dụng Bootstrap modal, bạn có thể triển khai JSInterop để hiện modal
            await _jsRuntime.InvokeVoidAsync("showCustomDialog", title, content.ToString());
        }
    }
}