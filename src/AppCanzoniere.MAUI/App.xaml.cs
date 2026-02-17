using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppCanzoniere.MAUI;

public partial class App : global::Microsoft.Maui.Controls.Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		// Initialize database
		InitializeDatabaseAsync(serviceProvider).ConfigureAwait(false);
	}

	private async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		
		// Apply migrations
		await dbContext.Database.MigrateAsync();
		
		// Seed initial data
		var seeder = new DatabaseSeeder(dbContext);
		await seeder.SeedAsync();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}