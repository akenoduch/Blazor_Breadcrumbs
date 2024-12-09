
# Blazor Generic Breadcrumb Component

This repository contains a **Generic Breadcrumb Component** for Blazor applications, enabling breadcrumb navigation to enhance user experience and track navigation history. The component uses `BreadcrumbService` for managing breadcrumb data and integrates with `JSInterop` for cookie-based persistence.

## Features

- **Dynamic Breadcrumb Management:** Tracks navigation changes and updates breadcrumb items dynamically.
- **Persistence with Cookies:** Stores breadcrumbs in cookies for state retention across sessions.
- **Customizable UI:** Easily styled breadcrumb interface.
- **Lightweight Integration:** Requires minimal setup to integrate into Blazor projects.

---

## Components and Files

### **`BreadcrumbService.cs`**
- Core service that:
  - Manages breadcrumb states and updates based on navigation events.
  - Persists breadcrumb data in cookies via `JSInterop`.
  - Handles breadcrumb truncation for maintaining relevance.

### **`Breadcrumbs.razor`**
- UI component for rendering the breadcrumb trail.
- Integrates with `BreadcrumbService` for dynamic updates.
- Supports customizable breadcrumb items and design.

### **`blazorExtensions.js`**
- JavaScript utilities for:
  - Managing cookies (`setCookie`, `getCookie`, `deleteCookie`).
  - Handling breadcrumb click navigation.
  - Observing DOM events for the breadcrumb component.

### **Layouts**
- **`App.razor`**
  - Entry point to the Blazor application, ensuring proper rendering.
- **`MainLayout.razor`**
  - Includes the JavaScript file (`blazorExtensions.js`) and manages layout-level URL updates.

---

## Installation and Setup

1. **Add the `BreadcrumbService` to the DI Container:**
   ```csharp
   builder.Services.AddScoped<BreadcrumbService>();
   ```

2. **Include the JavaScript File in Your Layout:**
   ```html
   <script src="js/blazorExtensions.js"></script>
   ```

3. **Inject the Breadcrumb Component in Your Page:**
   ```razor
   @inject BreadcrumbService BreadcrumbService

   <div id="breadcrumbs">
       <Breadcrumbs />
   </div>
   ```

4. **Add Breadcrumb Items in Your Page Logic:**
   ```razor
   protected override async Task OnInitializedAsync()
   {
       await BreadcrumbService.AddBreadcrumb("Breadcrumb Title", NavigationManager.Uri);
   }
   ```

---

## Styling

The breadcrumb component is styled using the provided CSS within `Breadcrumbs.razor`. Customize it as needed:
```css
.breadcrumb {
    padding: 0.75rem 1rem;
    background-color: transparent;
}

.breadcrumb-item a {
    text-decoration: none;
}

.breadcrumb-item.active {
    font-weight: bold;
}
```

---

## JavaScript Integration

The JavaScript utilities in `blazorExtensions.js` provide cookie management and event handling. Key functions:
- **Set a Cookie:**
  ```javascript
  blazorExtensions.setCookie("key", "value", 365);
  ```
- **Get a Cookie:**
  ```javascript
  blazorExtensions.getCookie("key");
  ```
- **Delete a Cookie:**
  ```javascript
  blazorExtensions.deleteCookie("key");
  ```

---

## Usage Example

```razor
@inject BreadcrumbService BreadcrumbService

@if (ShowBreadcrumbs)
{
    <Breadcrumbs />
}

protected override async Task OnInitializedAsync()
{
    await BreadcrumbService.AddBreadcrumb("Home", "/");
    await BreadcrumbService.AddBreadcrumb("Dashboard", "/dashboard");
}
```

---

## Roadmap

- [ ] Add support for custom separators between breadcrumb items.
- [ ] Improve accessibility with ARIA roles and attributes.
- [ ] Provide advanced customization options for the breadcrumb UI.

---

## Contributing

Feel free to open issues and submit pull requests for improvements or bug fixes. Contributions are welcome!

---

## Acknowledgments

Thanks to the Blazor and .NET community for their resources and contributions, enabling seamless development of this component.
