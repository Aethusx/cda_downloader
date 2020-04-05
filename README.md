# cda_downloader
very simple library to get .mp4 from cda.pl videos
# original decrypt code from player.js
```javascript
L = function(a) {
    for (var b = [], e = 0; e < a.length; e++) {
        var f = a.charCodeAt(e);
        b[e] = 33 <= f && 126 >= f ? String.fromCharCode(33 + (f + 14) % 94) : String.fromCharCode(f)
    }
    return da(b.join(""))
};
```
