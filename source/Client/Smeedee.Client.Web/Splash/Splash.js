function onSourceDownloadProgressChanged(sender, eventArgs) {
    var rectBar = sender.findName("rectBar");
    var rectBorder = sender.findName("rectBorder");
    if (eventArgs.progress)
        rectBar.Width = eventArgs.progress * rectBorder.Width;
    else
        rectBar.Width = eventArgs.get_progress() * rectBorder.Width;
}