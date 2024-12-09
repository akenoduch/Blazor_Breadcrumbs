using BlazorBreadcrumbs.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BreadcrumbService
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private bool _breadcrumbPresent;
    private DotNetObjectReference<BreadcrumbService> _objectReference;

    public event Action OnBreadcrumbsCleared;

    public BreadcrumbService(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _navigationManager.LocationChanged += OnLocationChanged;
        _objectReference = DotNetObjectReference.Create(this);
    }

    [JSInvokable]
    public void BreadcrumbsElementDetected()
    {
        Console.WriteLine("Breadcrumbs element detected.");
        _breadcrumbPresent = true;
    }

    private async void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        Console.WriteLine("Location changed to: " + e.Location);
        ResetBreadcrumbPresence();
        await Task.Delay(100);

        try
        {
            await _jsRuntime.InvokeVoidAsync("blazorExtensions.observeBreadcrumbsElement", _objectReference);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error invoking JavaScript function observeBreadcrumbsElement: " + ex.Message);
        }

        try
        {
            if (!await IsBreadcrumbsComponentPresent())
            {
                Console.WriteLine("Breadcrumbs component not present. Clearing cookies.");
                await ClearBreadcrumbsCookies();
                ResetBreadcrumbPresence();
                OnBreadcrumbsCleared?.Invoke();
            }
            else
            {
                Console.WriteLine("Breadcrumbs component is present.");
                await ResetBreadcrumbsIfNecessary();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error invoking JavaScript function checkForBreadcrumbsElement: " + ex.Message);
        }
    }

    private async Task ResetBreadcrumbsIfNecessary()
    {
        if (!_breadcrumbPresent)
        {
            Console.WriteLine("Resetting breadcrumbs because _breadcrumbPresent is false.");
            await ClearBreadcrumbsCookies();
            OnBreadcrumbsCleared?.Invoke();
        }
    }

    public async Task<bool> IsBreadcrumbsComponentPresent()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<bool>("blazorExtensions.checkForBreadcrumbsElement");
            Console.WriteLine("IsBreadcrumbsComponentPresent result: " + result);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error invoking JavaScript function checkForBreadcrumbsElement: " + ex.Message);
            return false;
        }
    }

    public void RegisterBreadcrumb()
    {
        Console.WriteLine("RegisterBreadcrumb called.");
        _breadcrumbPresent = true;
    }

    public void ResetBreadcrumbPresence()
    {
        Console.WriteLine("ResetBreadcrumbPresence called.");
        _breadcrumbPresent = false;
    }

    public async Task ClearBreadcrumbsCookies()
    {
        Console.WriteLine("ClearBreadcrumbsCookies called.");
        await _jsRuntime.InvokeVoidAsync("blazorExtensions.deleteCookie", "breadcrumbs");
    }

    public List<Breadcrumbs.BreadcrumbItem> GetBreadcrumbItems(string currentUri)
    {
        Console.WriteLine("GetBreadcrumbItems called with URI: " + currentUri);
        List<Breadcrumbs.BreadcrumbItem> breadcrumbs = new();

        if (string.IsNullOrEmpty(currentUri) || currentUri == "/")
        {
            breadcrumbs.Add(new Breadcrumbs.BreadcrumbItem { Title = "Home", Url = "/" });
            return breadcrumbs;
        }

        var uriWithoutDomain = new Uri(currentUri).AbsolutePath;
        var segments = uriWithoutDomain.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        breadcrumbs.Add(new Breadcrumbs.BreadcrumbItem { Title = "Home", Url = "/" });

        string url = "/";
        foreach (var segment in segments)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                url += segment + "/";
                breadcrumbs.Add(new Breadcrumbs.BreadcrumbItem { Title = segment, Url = url });
            }
        }

        return breadcrumbs;
    }

    public async Task SaveBreadcrumbsToCookies(string pageUri, List<Breadcrumbs.BreadcrumbItem> items)
    {
        Console.WriteLine("Saving breadcrumbs to cookies for page: " + pageUri);
        var itemsJson = System.Text.Json.JsonSerializer.Serialize(items);
        await _jsRuntime.InvokeVoidAsync("blazorExtensions.setCookie", pageUri + "-breadcrumbs", itemsJson, 7);
    }

    public async Task<List<Breadcrumbs.BreadcrumbItem>> LoadBreadcrumbsFromCookies(string pageUri)
    {
        Console.WriteLine("Loading breadcrumbs from cookies for page: " + pageUri);
        var savedItemsJson = await _jsRuntime.InvokeAsync<string>("blazorExtensions.getCookie", pageUri + "-breadcrumbs");
        if (!string.IsNullOrEmpty(savedItemsJson))
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<Breadcrumbs.BreadcrumbItem>>(savedItemsJson) ?? new List<Breadcrumbs.BreadcrumbItem>();
        }
        return new List<Breadcrumbs.BreadcrumbItem>();
    }

    public async Task SavePageState(string pageUri, Dictionary<string, object> state)
    {
        Console.WriteLine("Saving page state to cookies for page: " + pageUri);
        await _jsRuntime.InvokeVoidAsync("blazorExtensions.savePageState", pageUri, state);
    }

    public async Task<Dictionary<string, object>> LoadPageState(string pageUri)
    {
        Console.WriteLine("Loading page state from cookies for page: " + pageUri);
        var state = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("blazorExtensions.loadPageState", pageUri);
        return state ?? new Dictionary<string, object>();
    }
}
