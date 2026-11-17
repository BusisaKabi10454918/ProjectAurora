document.addEventListener("DOMContentLoaded", () => {
    const popup = document.getElementById("newPostPopup");
    const openButton = document.getElementById("newPostBtn");
    const cancelButton = document.getElementById("cancelPostBtn");

    if (!popup || !openButton || !cancelButton) return;

    openButton.addEventListener("click", async () => {
        try {
            const response = await fetch("?handler=CheckLogin");
            const result = await response.json();

            if (result.loggedIn) {
                popup.classList.add("active");
            }
            else {
                window.location.href = "/Login";
            }
        }
        catch (error) {
            console.error("Unable to verify login:", error);
            alert("Unable to verify your login. Please try again.");
        }
    });

    cancelButton.addEventListener("click", () => {
        popup.classList.remove("active");
    });
});