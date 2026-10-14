function toggleNavigationMenu() {
    var navigationLinksHolder = document.getElementById("top-menu-links")

    if (navigationLinksHolder.style.display == "block") {
        navigationLinksHolder.style.display = "none";
    } else {
        navigationLinksHolder.style.display = "block";
    }
}