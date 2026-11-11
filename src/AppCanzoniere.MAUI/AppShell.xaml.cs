using AppCanzoniere.MAUI.Pages;

namespace AppCanzoniere.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("songdetail", typeof(SongDetailPage));
        Routing.RegisterRoute("songmetadata", typeof(SongMetadataPage));
        Routing.RegisterRoute("aichords", typeof(AiChordSuggestionPage));
        Routing.RegisterRoute("albumdetail", typeof(AlbumDetailPage));
        Routing.RegisterRoute("cloudsync", typeof(CloudSyncPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("publicsongs", typeof(PublicSongsPage));
    }
}
