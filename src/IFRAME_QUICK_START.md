# Quick Start: Iframe Embedding

## ✅ Frontend - Already Configured!

The React application is **ready for iframe embedding** with:
- ✅ No frame-busting code
- ✅ Iframe detection utilities
- ✅ Parent window communication
- ✅ Auto-login for seamless embedding

## ⚠️ Backend - Requires Configuration

Update `/Backend/WhatsNewAPI/Program.cs` to allow iframe embedding by adding your parent application domain to the allowed origins list (already done in the file).

## Quick Test

### Test Locally

Create a file `test-iframe.html`:

```html
<!DOCTYPE html>
<html>
<head>
  <title>Iframe Test</title>
  <style>
    body { margin: 0; padding: 20px; font-family: Arial; }
    iframe { width: 100%; height: 90vh; border: 1px solid #ddd; }
    .info { padding: 10px; background: #f0f0f0; margin-bottom: 10px; }
  </style>
</head>
<body>
  <div class="info">
    <strong>Parent Application</strong> - Testing What's New iframe embedding
  </div>
  <iframe 
    src="http://localhost:5173" 
    title="What's New App"
  ></iframe>
  
  <script>
    // Listen for messages from What's New
    window.addEventListener('message', (event) => {
      console.log('📨 Message from iframe:', event.data);
    });
  </script>
</body>
</html>
```

Open this file in a browser while your app is running.

## Production Deployment

1. **Update CORS origins** in `Program.cs`:
   ```csharp
   policy.WithOrigins(
       "https://your-parent-app.company.com"
   )
   ```

2. **Update frame-ancestors** in `Program.cs`:
   ```csharp
   var allowedFrameOrigins = new[]
   {
       "'self'",
       "https://your-parent-app.company.com"
   };
   ```

3. **Deploy and test** with your actual parent application

## Integration Example

```html
<!-- In your parent application -->
<iframe 
  src="https://your-domain.com/whats-new"
  title="What's New"
  style="width: 100%; height: 100vh; border: none;"
></iframe>
```

## Troubleshooting

**Error: "Refused to display in a frame"**
→ Update Content-Security-Policy in Program.cs with your parent domain

**Error: CORS issues**
→ Add parent domain to CORS policy in Program.cs

**Cookies not working**
→ Ensure HTTPS in production (required for cross-origin cookies)

For detailed information, see `/IFRAME_EMBEDDING_GUIDE.md`
