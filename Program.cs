builder.Services.AddSignalR();

// Trong phần app configuration
app.MapHub<ChatHub>("/chatHub");
