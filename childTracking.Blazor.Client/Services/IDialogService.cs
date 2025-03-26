using Microsoft.AspNetCore.Components;

namespace childTracking.Blazor.Client.Services
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmation(string title, string message);
        Task ShowAlert(string title, string message);
        Task ShowCustomDialog(string title, MarkupString content);
    }
}