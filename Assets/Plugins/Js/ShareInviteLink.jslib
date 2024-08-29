mergeInto(LibraryManager.library, {
    ShareInviteLink: function(url) {
        var jsUrl = UTF8ToString(url);
        ShareInviteLink(jsUrl);
    }
});