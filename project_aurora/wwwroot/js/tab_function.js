const editor = document.getElementById("SourceCode");

editor.addEventListener("keydown", function (e) {
    if (e.key === "Tab") {
        e.preventDefault();

        const start = this.selectionStart;
        const end = this.selectionEnd;

        const tab = "    "; // 4 spaces

        this.value =
            this.value.substring(0, start) +
            tab +
            this.value.substring(end);

        this.selectionStart = this.selectionEnd = start + tab.length;
    }
});