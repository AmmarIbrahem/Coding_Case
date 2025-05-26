using PostsAPI.Data;
using PostsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();
builder.Services.AddScoped<IPostService, PostService>();

// Configure CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

// Configure JSON options
builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
	options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Seed some initial data
using (var scope = app.Services.CreateScope())
{
	var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
	await SeedData(postService);
}

app.Run();

static async Task SeedData(IPostService postService)
{
	var samplePosts = new[]
	{
		new PostsAPI.Models.PostInput
		{
			Author = "Jane Doe",
			Date = DateTime.Parse("2025-05-16T10:30:00Z"),
			Content = "First post content."
		},
		new PostsAPI.Models.PostInput
		{
			Author = "John Smith",
			Date = DateTime.Parse("2025-05-16T11:00:00Z"),
			Content = "Second post content."
		}
	};

	foreach (var post in samplePosts)
	{
		await postService.CreatePostAsync(post);
	}
}