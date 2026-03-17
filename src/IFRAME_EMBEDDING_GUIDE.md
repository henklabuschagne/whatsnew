# Iframe Embedding Configuration

This application is designed to be embedded in iframes for internal use. This document outlines the necessary configurations.

## Frontend Configuration

The React frontend application is **fully compatible** with iframe embedding:

✅ **No frame-busting scripts** - The app does not prevent iframe embedding
✅ **Cross-origin compatible** - Works with different origins (with proper backend setup)
✅ **Parent communication** - Includes utilities to communicate with parent window
✅ **Auto-detection** - Automatically detects when running in an iframe

### Iframe Utilities

The app includes `/utils/iframe.ts` with the following utilities:

- `isInIframe()` - Detects if running in an iframe
- `notifyParentReady()` - Notifies parent window when app is ready
- `notifyParentNavigation(path)` - Notifies parent of navigation changes
- `sendMessageToParent(message)` - Sends custom messages to parent
- `onMessageFromParent(callback)` - Listens for messages from parent

### Example: Embedding the App

```html
<!-- In your parent application -->
<!DOCTYPE html>
<html>
<head>
  <title>Parent Application</title>
  <style>
    #whats-new-iframe {
      width: 100%;
      height: 100vh;
      border: none;
    }
  </style>
</head>
<body>
  <iframe 
    id="whats-new-iframe"
    src="https://your-domain.com/whats-new"
    title="What's New Application"
    allow="fullscreen"
  ></iframe>

  <script>
    // Listen for messages from the What's New app
    window.addEventListener('message', (event) => {
      // Validate origin for security
      // if (event.origin !== 'https://your-domain.com') return;
      
      console.log('Message from What\'s New:', event.data);
      
      if (event.data.type === 'WHATS_NEW_READY') {
        console.log('What\'s New app is ready!');
      }
      
      if (event.data.type === 'WHATS_NEW_NAVIGATION') {
        console.log('User navigated to:', event.data.path);
      }
    });
  </script>
</body>
</html>
```

## Backend Configuration (.NET Core API)

**IMPORTANT**: The backend needs to be configured to allow iframe embedding.

### Required Changes to Program.cs

Update the security headers middleware to allow iframe embedding from your internal domain:

```csharp
// Before (blocks all iframe embedding):
context.Response.Headers.Add("X-Frame-Options", "DENY");

// After (allows iframe embedding from your domain):
context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN"); // For same origin
// OR
context.Response.Headers.Add("X-Frame-Options", "ALLOW-FROM https://your-parent-domain.com"); // For specific domain

// Recommended for modern browsers - use Content-Security-Policy instead:
context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://your-parent-domain.com");
```

### Content Security Policy (CSP) Configuration

For better security and compatibility, use CSP instead of X-Frame-Options:

```csharp
app.Use(async (context, next) =>
{
    // Add security headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // Allow iframe embedding from specific origins
    var allowedOrigins = new[]
    {
        "https://your-internal-app.company.com",
        "https://another-internal-app.company.com"
    };
    
    var frameAncestors = string.Join(" ", allowedOrigins);
    context.Response.Headers.Add("Content-Security-Policy", 
        $"frame-ancestors 'self' {frameAncestors}");
    
    await next();
});
```

### CORS Configuration

Ensure CORS is properly configured if the parent app is on a different domain:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInternalApps", policy =>
    {
        policy.WithOrigins(
            "https://your-internal-app.company.com",
            "https://another-internal-app.company.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// In the middleware pipeline:
app.UseCors("AllowInternalApps");
```

## Testing Iframe Embedding

### Local Testing

Create a simple HTML file to test:

```html
<!DOCTYPE html>
<html>
<head>
  <title>Iframe Test</title>
</head>
<body>
  <h1>Testing What's New Iframe Embedding</h1>
  <iframe 
    src="http://localhost:5173" 
    width="1200" 
    height="800" 
    title="What's New App"
  ></iframe>
</body>
</html>
```

### Browser Console Test

In the parent page console:

```javascript
// Check if iframe loaded successfully
const iframe = document.querySelector('iframe');
console.log('Iframe loaded:', iframe.contentWindow !== null);

// Send a test message to the iframe
iframe.contentWindow.postMessage({ type: 'TEST' }, '*');
```

## Security Considerations

1. **Origin Validation**: Always validate message origins in both parent and child windows
2. **Whitelist Origins**: Only allow embedding from trusted internal domains
3. **HTTPS**: Use HTTPS in production for secure communication
4. **CSP**: Use Content-Security-Policy header instead of deprecated X-Frame-Options
5. **Cookie SameSite**: Set appropriate SameSite cookie attributes if using authentication

## Environment Variables

For the frontend, you can add optional environment variables:

```env
# Optional: Comma-separated list of allowed parent origins
VITE_ALLOWED_PARENT_ORIGINS=https://app1.company.com,https://app2.company.com

# Optional: Enable iframe mode features
VITE_IFRAME_MODE=true
```

## Common Issues and Solutions

### Issue: "Refused to display in a frame"
**Solution**: Update backend X-Frame-Options or Content-Security-Policy headers

### Issue: Cross-origin errors
**Solution**: Configure CORS properly on the backend

### Issue: Cookies not working in iframe
**Solution**: Set `SameSite=None; Secure` on cookies (requires HTTPS)

### Issue: Parent can't communicate with iframe
**Solution**: Ensure both are using `postMessage` correctly and validating origins

## Production Deployment

1. Update backend headers to allow your production parent domain(s)
2. Update CORS policy with production origins
3. Test thoroughly in production environment
4. Monitor for any CSP violations in browser console
5. Document the parent application integration for your team

## Additional Resources

- [MDN: X-Frame-Options](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options)
- [MDN: Content-Security-Policy frame-ancestors](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/frame-ancestors)
- [MDN: Window.postMessage()](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage)
