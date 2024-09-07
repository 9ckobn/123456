mergeInto(LibraryManager.library, {
        OpenUrl: function(url) {
            var jsUrl = UTF8ToString(url);
            OpenUrl(jsUrl);
        }
});