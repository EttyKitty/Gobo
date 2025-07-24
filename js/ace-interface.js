window.aceEditors = window.aceEditors || {};

window.editorRender = function(element, mode, theme, readOnly, tabSize) {
    let editor = window.aceEditors[element];
    if (!editor) {
        editor = ace.edit(element);
        window.aceEditors[element] = editor;
    }

    var defineMode = "ace/mode/" + mode;
    editor.setTheme("ace/theme/" + theme);

    editor.setReadOnly(readOnly);
    editor.session.setMode(defineMode);
    editor.session.setTabSize(tabSize);
    editor.renderer.setScrollMargin(10, 10);
    editor.commands.removeCommands(["openCommandPalette", "showSettingsMenu"]);

    editor.setOptions({
        autoScrollEditorIntoView: true
    });

    editor.setShowPrintMargin(true);
    editor.setHighlightActiveLine(false);
};

window.ace_destroy = function(element) {
    var editor = window.aceEditors[element];
    if (editor) {
        editor.destroy();
        editor.container.remove();
        delete window.aceEditors[element];
    }
};

window.ace_set_readonly = function(element, readOnly) {
    var editor = window.aceEditors[element];
    if (editor) {
        editor.setReadOnly(readOnly);
    }
};

window.GetCode = function(dotNetHelper, element) {
    var editor = window.aceEditors[element];
    if (editor) {
        var code = editor.getSession().getValue();
        dotNetHelper.invokeMethodAsync('ReceiveCode', code);
    }
};

window.SetCode = function (dotNetHelper, element, code) {
    var editor = window.aceEditors[element];
    if (editor) {
        editor.getSession().setValue(code);
        editor.renderer.updateFull();
        dotNetHelper.invokeMethodAsync('ReceiveCode', code);
    }
};

window.SetWidth = function (element, width) {
    var editor = window.aceEditors[element];
    if (editor) {
        if (width > 0) {
            editor.setOption("printMarginColumn", width);
        }
    }
};

window.setShowPrintMargin = (editorId, show) => {
    const editor = window.aceEditors[editorId];
    if (editor) {
        editor.setShowPrintMargin(show);
    }
};