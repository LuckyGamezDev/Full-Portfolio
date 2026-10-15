function toggleNavigationMenu() {
    var navigationLinksHolder = document.getElementById("top-menu-links")

    if (navigationLinksHolder.style.display == "flex") {
        navigationLinksHolder.style.display = "none";
    } else {
        navigationLinksHolder.style.display = "flex";
    }
}