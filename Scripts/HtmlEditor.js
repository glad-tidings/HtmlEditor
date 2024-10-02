Type.registerNamespace("ParsElecom.HtmlEditor");

ParsElecom.HtmlEditor.HtmlEditor = function(element)
{
    ParsElecom.HtmlEditor.HtmlEditor.initializeBase(this, [element]);
    
    this._htmlencodedTextID = "";
    this._modifiedID = "";
    this._htmlEditorID = "";
    this._designEditorID = "";
    this._htmlAreaID = "";
    this._designAreaID = "";
    this._htmlTabID = "";
    this._designTabID = "";
    this._dialogBoxIDs = "";
    this._saveButtonID = "";
    this._toolbarIDs = "";
    this._backColor = "";
    this._editorForeColor = "";
    this._editorBackColor = "";
    this._toolbarColor = "";
    this._tabForeColor = "";
    this._tabBackColor = "";
    this._selectedTabBackColor = "";
    this._selectedTabTextColor = "";
    this._tabMouseOverColor = "";
    this._tabbarBackColor = "";
    this._buttonMouseOverColor = "";
    this._buttonMouseOverBorderColor = "";
    this._dialogColors = "";
    this._outputXHTML = "";
    this._convertDeprecatedSyntax = "";
    this._convertParagraphs = "";
    this._replaceNoBreakSpace = "";
    this._showModifiedAsterick = "";
    this._toolbarData = "";
    this._dialogData = "";
    this._dialogDimensions = "";
    this._pixelImageUrl = "";
    this._allowedTags = "";
    this._allowedAttributes = "";
    this._noScriptAttributes = "";
    this._formatHtmlMode = false;
    this._newLine = "";
    this._initialMode = "";
    this._designModeEditable = false;
    this._htmlModeEditable = false;
    this._designModeCss = "";
    this._designModeEmulateIE7 = false;
    this._textDirection = "";
    this._validatorIDs = null;
    this._version = "";
    this._modified = false;
    this.htmlEditor = null;
    this.designEditor = null;
    this.htmlArea = null;
    this.designArea = null;
    this.designModeDocument = null;
    this.htmlModeDocument = null;
    this.designModeFocusTarget = null;
    this.designModeFocusElement = null;
    this.htmlModeFocusElement = null;
    this.dialogFrameElement = null;
    this.waitElement = null;
    this.designMode = false;
    this.cssCommand = "";
    this.outputXHTML = false;
    this.convertDeprecatedSyntax = false;
    this.convertParagraphs = false;
    this.replaceNoBreakSpace = false;
    this.dialogInfo = null;
    this.toolbarInfo = null;
    this.allowedTags = null;
    this.allowedAttributes = null;
    this.noScriptAttributes = null;
    this.newLine = "";
    this.noEndTags = null;
    this.blockTags = null;
    this.listTags = null;
    this.conversionRules = null;
    this.fontSizes = "";
    this.onDesignEditorKeypressHandler = null;
    this.onDesignEditorKeydownHandler = null;
    this.onDesignEditorMousedownHandler = null;
    this.onDesignEditorBlurHandler = null;
    this.onHtmlEditorBlurHandler = null;
    this.onEditorFocusHandler = null;
    this.onDialogFrameLoadHandler = null;
    this.endRequestHandler = null;
    this.timer = null;
    this.recordedHtml = "";
    this.innerHtml = "";
    this.initialized = false;
    this.designModeInitializedCount = 0;
    this.designModeInitialized = false;
    this.isIE = false;
    this.isFirefox = false;
    this.isOpera = false;
    this.browserVersion = 0;
    this.standardsMode = false;

    this.modalState = null;
}

ParsElecom.HtmlEditor.HtmlEditor.prototype =
{
    GetVersion: function()
    {
        return this.get_version();
    },

    Save: function()
    {
        var html = this.GetInnerHtml();
        if (this.outputXHTML) html = this.ConvertHTML(html, true);

        this.SetValue(html, false);

        var encodedhtml = this.ClientEncode(html);
        var htmlencodedText = $get(this.get_htmlencodedTextID());
        htmlencodedText.value = encodedhtml;

        this.RecordHtml();
    },

    IsModified: function()
    {
        return this.get_modified();
    },

    SaveIfModified: function()
    {
        if (this.IsModified()) this.Save();
    },

    GetText: function()
    {
        var html = this.GetInnerHtml();
        if (this.outputXHTML) html = this.ConvertHTML(html, true);

        return html;
    },

    SetText: function(html)
    {
        var encodedHtml = this.ClientEncode(html);

        var htmlencodedText = $get(this.get_htmlencodedTextID());
        htmlencodedText.value = encodedHtml;

        this.UpdateHtml();
    },

    SetValue: function(html, convertToXHTML)
    {
        if (this.outputXHTML && convertToXHTML)
            html = this.ConvertHTML(html, true);

        this.get_element().value = html;
    },

    SetMode: function(mode)
    {
        this.CloseDialogBox();

        if (mode.toLowerCase() == "design")
        {
            this.GoToDesignMode();
        }
        else if (mode.toLowerCase() == "html")
        {
            this.GoToHtmlMode();
        }
    },

    GetMode: function()
    {
        return this.designMode ? "Design" : "Html";
    },

    EnableToolbar: function(toolbarName, enabled)
    {
        this.EnableToolbarElement('Toolbar', toolbarName, enabled);
    },

    EnableButtons: function(buttonBarName, enabled)
    {
        this.EnableToolbarElement('Buttons', buttonBarName, enabled);
    },

    EnableSelect: function(selectName, enabled)
    {
        this.EnableToolbarElement('Select', selectName, enabled);
    },

    EnableButton: function(buttonName, enabled)
    {
        this.EnableToolbarElement('Button', buttonName, enabled);
    },

    GetContext: function()
    {
        return this.designMode ? this.GetDesignModeContext() : null;
    },

    ExecuteCommand: function(commandName, commandValue)
    {
        if (!this.designMode) return;
        if (!this.designModeInitialized) return;
        if (!this.IsDesignModeSet()) return;
        if (!this.IsCommandSupported(commandName)) return;

        this.designModeFocusTarget.focus();

        var execute = false;
        var resetFocus = false;

        switch (commandName)
        {
            case "inserthorizontalrule":
                execute = !this.ExecuteRule();
                break;
            case "insertorderedlist":
            case "insertunorderedlist":
                execute = true;
                resetFocus = this.isIE && this.designModeDocument.queryCommandEnabled(commandName, false, commandValue);
                break;
            case "inserthtml":
                var context = this.GetDesignModeContext();
                if (context && this.isIE)
                    this.PasteHtml(context.range, commandValue);
                else if (context && this.isFirefox)
                    context.range.deleteContents();
                execute = !this.isIE;
                break;
            default:
                execute = true;
                break;
        }

        if (execute)
            this.designModeDocument.execCommand(commandName, false, commandValue);

        this.StoreDesignEditor();

        if (resetFocus)
            this.designEditor.focus();

        this.TimeoutInvoke(this.RaiseContextChanged);
    },

    get_htmlencodedTextID: function()
    {
        return this._htmlencodedTextID;
    },

    set_htmlencodedTextID: function(value)
    {
        this._htmlencodedTextID = value;
    },

    get_modifiedID: function()
    {
        return this._modifiedID;
    },

    set_modifiedID: function(value)
    {
        this._modifiedID = value;
    },

    get_htmlEditorID: function()
    {
        return this._htmlEditorID;
    },

    set_htmlEditorID: function(value)
    {
        this._htmlEditorID = value;
    },

    get_designEditorID: function()
    {
        return this._designEditorID;
    },

    set_designEditorID: function(value)
    {
        this._designEditorID = value;
    },

    get_htmlAreaID: function()
    {
        return this._htmlAreaID;
    },

    set_htmlAreaID: function(value)
    {
        this._htmlAreaID = value;
    },

    get_designAreaID: function()
    {
        return this._designAreaID;
    },

    set_designAreaID: function(value)
    {
        this._designAreaID = value;
    },

    get_htmlTabID: function()
    {
        return this._htmlTabID;
    },

    set_htmlTabID: function(value)
    {
        this._htmlTabID = value;
    },

    get_designTabID: function()
    {
        return this._designTabID;
    },

    set_designTabID: function(value)
    {
        this._designTabID = value;
    },

    get_dialogBoxIDs: function()
    {
        return this._dialogBoxIDs;
    },

    set_dialogBoxIDs: function(value)
    {
        this._dialogBoxIDs = value;
    },

    get_saveButtonID: function()
    {
        return this._saveButtonID;
    },

    set_saveButtonID: function(value)
    {
        this._saveButtonID = value;
    },

    get_toolbarIDs: function()
    {
        return this._toolbarIDs;
    },

    set_toolbarIDs: function(value)
    {
        this._toolbarIDs = value;
    },

    get_backColor: function()
    {
        return this._backColor;
    },

    set_backColor: function(value)
    {
        this._backColor = value;
    },

    get_editorForeColor: function()
    {
        return this._editorForeColor;
    },

    set_editorForeColor: function(value)
    {
        this._editorForeColor = value;
    },

    get_editorBackColor: function()
    {
        return this._editorBackColor;
    },

    set_editorBackColor: function(value)
    {
        this._editorBackColor = value;
    },

    get_toolbarColor: function()
    {
        return this._toolbarColor;
    },

    set_toolbarColor: function(value)
    {
        this._toolbarColor = value;
    },

    get_tabForeColor: function()
    {
        return this._tabForeColor;
    },

    set_tabForeColor: function(value)
    {
        this._tabForeColor = value;
    },

    get_tabBackColor: function()
    {
        return this._tabBackColor;
    },

    set_tabBackColor: function(value)
    {
        this._tabBackColor = value;
    },

    get_selectedTabBackColor: function()
    {
        return this._selectedTabBackColor;
    },

    set_selectedTabBackColor: function(value)
    {
        this._selectedTabBackColor = value;
    },

    get_selectedTabTextColor: function()
    {
        return this._selectedTabTextColor;
    },

    set_selectedTabTextColor: function(value)
    {
        this._selectedTabTextColor = value;
    },

    get_tabMouseOverColor: function()
    {
        return this._tabMouseOverColor;
    },

    set_tabMouseOverColor: function(value)
    {
        this._tabMouseOverColor = value;
    },

    get_tabbarBackColor: function()
    {
        return this._tabbarBackColor;
    },

    set_tabbarBackColor: function(value)
    {
        this._tabbarBackColor = value;
    },

    get_buttonMouseOverColor: function()
    {
        return this._buttonMouseOverColor;
    },

    set_buttonMouseOverColor: function(value)
    {
        this._buttonMouseOverColor = value;
    },

    get_buttonMouseOverBorderColor: function()
    {
        return this._buttonMouseOverBorderColor;
    },

    set_buttonMouseOverBorderColor: function(value)
    {
        this._buttonMouseOverBorderColor = value;
    },

    get_dialogColors: function()
    {
        return this._dialogColors;
    },

    set_dialogColors: function(value)
    {
        this._dialogColors = value;
    },

    get_outputXHTML: function()
    {
        return this._outputXHTML;
    },

    set_outputXHTML: function(value)
    {
        this._outputXHTML = value;
    },

    get_convertDeprecatedSyntax: function()
    {
        return this._convertDeprecatedSyntax;
    },

    set_convertDeprecatedSyntax: function(value)
    {
        this._convertDeprecatedSyntax = value;
    },

    get_convertParagraphs: function()
    {
        return this._convertParagraphs;
    },

    set_convertParagraphs: function(value)
    {
        this._convertParagraphs = value;
    },

    get_replaceNoBreakSpace: function()
    {
        return this._replaceNoBreakSpace;
    },

    set_replaceNoBreakSpace: function(value)
    {
        this._replaceNoBreakSpace = value;
    },

    get_showModifiedAsterick: function()
    {
        return this._showModifiedAsterick;
    },

    set_showModifiedAsterick: function(value)
    {
        this._showModifiedAsterick = value;
    },

    get_toolbarData: function()
    {
        return this._toolbarData;
    },

    set_toolbarData: function(value)
    {
        this._toolbarData = value;
    },

    get_dialogData: function()
    {
        return this._dialogData;
    },

    set_dialogData: function(value)
    {
        this._dialogData = value;
    },

    get_dialogDimensions: function()
    {
        return this._dialogDimensions;
    },

    set_dialogDimensions: function(value)
    {
        this._dialogDimensions = value;
    },

    get_pixelImageUrl: function()
    {
        return this._pixelImageUrl;
    },

    set_pixelImageUrl: function(value)
    {
        this._pixelImageUrl = value;
    },

    get_allowedTags: function()
    {
        return this._allowedTags;
    },

    set_allowedTags: function(value)
    {
        this._allowedTags = value;
    },

    get_allowedAttributes: function()
    {
        return this._allowedAttributes;
    },

    set_allowedAttributes: function(value)
    {
        this._allowedAttributes = value;
    },

    get_noScriptAttributes: function()
    {
        return this._noScriptAttributes;
    },

    set_noScriptAttributes: function(value)
    {
        this._noScriptAttributes = value;
    },

    get_formatHtmlMode: function()
    {
        return this._formatHtmlMode;
    },

    set_formatHtmlMode: function(value)
    {
        this._formatHtmlMode = value;
    },

    get_newLine: function()
    {
        return this._newLine;
    },

    set_newLine: function(value)
    {
        this._newLine = value;
    },

    get_initialMode: function()
    {
        return this._initialMode;
    },

    set_initialMode: function(value)
    {
        this._initialMode = value;
    },

    get_designModeEditable: function()
    {
        return this._designModeEditable;
    },

    set_designModeEditable: function(value)
    {
        this._designModeEditable = value;
    },

    get_htmlModeEditable: function()
    {
        return this._htmlModeEditable;
    },

    set_htmlModeEditable: function(value)
    {
        this._htmlModeEditable = value;
    },

    get_designModeCss: function()
    {
        return this._designModeCss;
    },

    set_designModeCss: function(value)
    {
        this._designModeCss = value;
    },

    get_designModeEmulateIE7: function()
    {
        return this._designModeEmulateIE7;
    },

    set_designModeEmulateIE7: function(value)
    {
        this._designModeEmulateIE7 = value;
    },

    get_textDirection: function()
    {
        return this._textDirection;
    },

    set_textDirection: function(value)
    {
        this._textDirection = value;
    },

    get_validatorIDs: function()
    {
        return this._validatorIDs;
    },

    set_validatorIDs: function(value)
    {
        this._validatorIDs = value;
    },

    get_version: function()
    {
        return this._version;
    },

    set_version: function(value)
    {
        this._version = value;
    },

    get_modified: function()
    {
        return this._modified;
    },

    set_modified: function(value)
    {
        if (this._modified == value) return;

        this._modified = value;
        $get(this.get_modifiedID()).value = value ? "true" : "false";

        if (this.initialized)
        {
            this.SetModifiedIndicators();
            this.RaiseModifiedChanged();
        }

        this.raisePropertyChanged('modified');
    },

    IsCommandSupported: function(commandName)
    {
        switch (commandName)
        {
            case "inserthtml":
                if (this.isIE) return true;
                break;
            default:
                break;
        }

        try
        {
            this.designModeDocument.queryCommandEnabled(commandName);
            return true;
        }
        catch (e)
        {
            return false;
        }
    },

    IsDesignModeSet: function()
    {
        return this.designModeDocument.designMode.toLowerCase() == "on";
    },

    PasteHtml: function(range, html)
    {
        if (range.pasteHTML)
        {
            try
            {
                range.pasteHTML(html)
            }
            catch (e)
            {
                return;
            }
        }
    },

    DoBold: function()
    {
        this.ExecuteCommand('bold', null);
    },

    DoItalic: function()
    {
        this.ExecuteCommand('italic', null);
    },

    DoUnderline: function()
    {
        this.ExecuteCommand('underline', null);
    },

    DoLeft: function()
    {
        this.ExecuteCommand('justifyleft', null);
    },

    DoCenter: function()
    {
        this.ExecuteCommand('justifycenter', null);
    },

    DoRight: function()
    {
        this.ExecuteCommand('justifyright', null);
    },

    DoJustify: function()
    {
        this.ExecuteCommand('justifyfull', null);
    },

    DoOrderedList: function()
    {
        this.ExecuteCommand('insertorderedlist', null);
    },

    DoBulletedList: function()
    {
        this.ExecuteCommand('insertunorderedlist', null);
    },

    DoIndent: function()
    {
        this.ExecuteCommand('indent', null);
    },

    DoOutdent: function()
    {
        this.ExecuteCommand('outdent', null);
    },

    ExecuteRule: function()
    {
        if (!this.isIE) return false;

        var sel = this.designModeDocument.selection;
        var r = sel.createRange();
        if (r.parentElement().tagName.toLowerCase() != "p") return false;

        r.collapse(true);
        r.select();
        this.PasteHtml(r, "<hr />");

        return true;
    },

    DoRule: function()
    {
        this.ExecuteCommand('inserthorizontalrule', null);
    },

    DoSubscript: function()
    {
        this.ExecuteCommand('subscript', null);
    },

    DoSuperscript: function()
    {
        this.ExecuteCommand('superscript', null);
    },

    DoFont: function(fontName)
    {
        if (fontName != '')
            this.ExecuteCommand('fontname', fontName);
    },

    DoSize: function(fontSize)
    {
        if (fontSize != '')
            this.ExecuteCommand('fontsize', fontSize);
    },

    DoFormat: function(formatType)
    {
        if (formatType != '')
            this.ExecuteCommand('formatblock', '<' + formatType + '>');
    },

    ButtonSwitchMode: function(buttonName, mode)
    {
        var button = $get(this.FindToolbarElement('Button', buttonName).id);
        this.MouseOut(button);
        this.SwitchMode(button, mode);
    },

    DoDesign: function()
    {
        this.ButtonSwitchMode('Design', 'Design');
    },

    DoHtml: function()
    {
        this.ButtonSwitchMode('Html', 'Html');
    },

    DoView: function()
    {
        this.ButtonSwitchMode('View', 'Design');
    },

    DoSave: function()
    {
        if (this.IsModified())
        {
            var toolbarButton = $get(this.FindToolbarElement("Button", "Save").id);
            this.MouseOut(toolbarButton);

            toolbarButton.style.cursor = "wait";
            this.Save();
            toolbarButton.style.cursor = "pointer";

            var saveButton = $get(this.get_saveButtonID());
            saveButton.click();

            if (this.isIE) this.SetFocus();
        }
    },

    DoNew: function()
    {
        if (confirm("Clear editor text?"))
            this.SetText("");
        else if (this.isIE)
            this.SetFocus();
    },

    FindDialogInfo: function(name)
    {
        for (var i = 0; i < this.dialogInfo.dialogs.length; i++)
        {
            if (this.dialogInfo.dialogs[i].name == name) return this.dialogInfo.dialogs[i];
        }

        return null;
    },

    ContainsTabElement: function(tabElements, tabElement)
    {
        for (var i = 0; i < tabElements.length; i++)
        {
            if (tabElements[i].e == tabElement) return true;
        }

        return false;
    },

    GetTabElements: function(parentElement, excludeElements)
    {
        var tabElements = new Array();
        var tabIndexTags = "a,area,button,input,object,select,textarea,iframe".split(',');

        var tabIndexElements = new Array();
        for (var i = 0; i < tabIndexTags.length; i++)
        {
            var elements = parentElement.getElementsByTagName(tabIndexTags[i]);
            if (elements == null || elements.length == 0) continue;

            for (var ii = 0; ii < elements.length; ii++)
            {
                if (excludeElements && this.ContainsTabElement(excludeElements, elements[ii])) continue;

                var o = { e: elements[ii], index: elements[ii].tabIndex, disabled: elements[ii].disabled, visibility: elements[ii].style.visibility };
                Array.add(tabElements, o);
            }
        }

        return tabElements;
    },

    DisableTabElements: function(parentElement, excludeElements)
    {
        var tabElements = this.GetTabElements(parentElement, excludeElements);

        for (var i = 0; i < tabElements.length; i++)
        {
            tabElements[i].tabIndex = -1;
            tabElements[i].e.disabled = true;

            if (this.isIE && this.browserVersion < 7 && tabElements[i].e.tagName.toLowerCase() == "select")
                tabElements[i].e.style.visibility = "hidden";
        }

        return tabElements;
    },

    RestoreTabElements: function(tabElements)
    {
        for (var i = 0; i < tabElements.length; i++)
        {
            tabElements[i].e.tabIndex = tabElements[i].index;
            tabElements[i].e.disabled = tabElements[i].disabled;

            if (this.isIE && this.browserVersion < 7 && tabElements[i].e.tagName.toLowerCase() == "select")
                tabElements[i].e.style.visibility = tabElements[i].visibility;
        }
    },

    CreateShield: function(parentElement, h, w)
    {
        var shield = document.createElement("div");
        shield.style.position = "absolute";
        shield.style.left = "0px";
        shield.style.top = "0px";
        shield.style.height = "0px";
        shield.style.width = "0px";
        shield.style.backgroundColor = "red";

        if (this.isIE)
            shield.style.filter = "alpha(opacity=0)";
        else
            shield.style.opacity = "0";

        parentElement.appendChild(shield);

        shield.style.height = h + "px";
        shield.style.width = w + "px";

        return shield;
    },

    RemoveShield: function(shield)
    {
        if (shield)
            shield.parentNode.removeChild(shield);

        return null;
    },

    SetShield: function(shield)
    {
        if (shield)
        {
            var editorElement = this.get_element();
            var dialogElement = $get(this.dialogInfo.elementIDs.dialogBox);

            this.modalState = new Object();
            this.modalState.tabElements = this.DisableTabElements(editorElement, this.GetTabElements(dialogElement));
            this.modalState.modalShield = this.CreateShield($get(this.dialogInfo.elementIDs.dialogShim), editorElement.offsetHeight, editorElement.offsetWidth);
            this.modalState.toolbarShield = null;

            if (this.toolbarInfo.toolbars.length > 0)
            {
                var toolbarContainer = $get(this.toolbarInfo.toolbars[0].id).parentNode;
                if (toolbarContainer.style.position.toLowerCase() == "absolute")
                    this.modalState.toolbarShield = this.CreateShield(toolbarContainer, toolbarContainer.offsetHeight, toolbarContainer.offsetWidth);
            }
        }
        else
        {
            this.modalState.toolbarShield = this.RemoveShield(this.modalState.toolbarShield);
            this.modalState.modalShield = this.RemoveShield(this.modalState.modalShield);
            this.RestoreTabElements(this.modalState.tabElements);

            this.modalState = null;
        }
    },

    GetCurrentDialogInfo: function(name)
    {
        var d = new Object();
        d.name = name;
        d.context = this.GetContext();

        return d;
    },

    GetDialogHeadingHtml: function(dialog)
    {
        var html = "";

        var toolbarElement = this.FindToolbarElement('Button', dialog.name);
        if (toolbarElement)
        {
            var button = $get(toolbarElement.id);
            var src = button.src;
            var alt = button.alt;
            var title = button.title;

            html = "<img style='position: absolute; left: 6px; top: 4px;' src='" + src + "' alt='" + alt + "' title='" + title + "' />";
        }

        html += dialog.heading;

        return html;
    },

    DialogBox: function(name)
    {
        var dialog = this.FindDialogInfo(name);
        if (dialog == null) return;

        if (this.designMode)
            this.designModeFocusTarget.focus();

        this.dialogInfo.current = this.GetCurrentDialogInfo(name);
        this.SetShield(true);

        var f = $get(this.dialogInfo.elementIDs.dialogFrame);
        var hh = Number(this.dialogInfo.options.dimensions.headingHeight) + 1;
        var bh = Number(this.dialogInfo.options.dimensions.buttonbarHeight) + 1;

        f.height = (Number(dialog.height) - hh - bh) + "px";

        var doc = f.contentDocument || f.contentWindow.document || f.document;
        doc.location.replace(dialog.frameUrl);

        var mh = $get(this.dialogInfo.elementIDs.dialogHeading);
        mh.innerHTML = this.GetDialogHeadingHtml(dialog);

        var md = $get(this.dialogInfo.elementIDs.dialogBox);
        md.style.height = dialog.height + "px";
        md.style.width = dialog.width + "px";

        var editorElement = this.get_element();
        var location = this.GetLocation(editorElement);

        var left = (editorElement.offsetWidth - dialog.width) / 2;
        left = Math.max(left, -location.x);
        md.style.left = left + "px";

        var top = (editorElement.offsetHeight - dialog.height) / 2;
        top = Math.max(top, -location.y);
        md.style.top = top + "px";

        md.style.display = "block";
    },

    CloseDialogBox: function()
    {
        if (!this.dialogInfo.current) return;

        var f = $get(this.dialogInfo.elementIDs.dialogFrame);

        if (f.contentWindow && f.contentWindow.OnClose)
        {
            f.contentWindow.OnClose();
        }

        if (this.dialogInfo.current.behavior)
        {
            this.dialogInfo.current.behavior.dispose();
            this.dialogInfo.current.behavior = null;
        }

        $get(this.dialogInfo.elementIDs.dialogBox).style.display = "none";
        this.dialogInfo.current.context = null;

        var blank = this.FindDialogInfo("Blank");
        if (blank)
        {
            this.dialogInfo.current.name = blank.name;
            var doc = f.contentDocument || f.contentWindow.document || f.document;
            doc.location.replace(blank.frameUrl);
        }

        this.SetShield(false);
        this.dialogInfo.current = null;
    },

    OnDialogFrameLoad: function(event)
    {
        if (!this.dialogInfo.current) return;

        var f = $get(this.dialogInfo.elementIDs.dialogFrame);

        if (f.contentWindow)
        {
            f.contentWindow.focus();

            if (f.contentWindow.Initialize)
            {
                var dialogDocument = f.contentDocument || f.contentWindow.document || f.document;
                var behavior = $create(ParsElecom.HtmlEditor.DialogBehavior, { "editorControl": this, "dialogDocument": dialogDocument, "contentWindow": f.contentWindow }, null, null, this.get_element());
                this.dialogInfo.current.behavior = behavior;

                f.contentWindow.Initialize(behavior);
            }
        }
    },

    ResetSelectedRange: function(range)
    {
        if (this.isIE)
        {
            range.select();
        }
        else
        {
            var selection = this.designEditor.getSelection();
            if (selection.rangeCount > 0) selection.removeAllRanges();

            selection.addRange(range);
        }
    },

    OnDialogOK: function()
    {
        if (this.dialogInfo.current.context != null && !this.isOpera)
        {
            this.ResetSelectedRange(this.dialogInfo.current.context.range);
            this.dialogInfo.current.behavior.UpdateContext();
        }

        var f = $get(this.dialogInfo.elementIDs.dialogFrame);

        if (f.contentWindow && f.contentWindow.OnOK)
        {
            f.contentWindow.OnOK();
        }

        this.CloseDialogBox();
    },

    Drag: function(elementToDrag, event)
    {
        var startX = event.clientX, startY = event.clientY;
        var origX = elementToDrag.offsetLeft, origY = elementToDrag.offsetTop;
        var deltaX = startX - origX, deltaY = startY - origY;

        if (document.addEventListener)
        {
            document.addEventListener("mousemove", moveHandler, true);
            document.addEventListener("mouseup", upHandler, true);
        }
        else if (document.attachEvent)
        {
            elementToDrag.setCapture();
            elementToDrag.attachEvent("onmousemove", moveHandler);
            elementToDrag.attachEvent("onmouseup", upHandler);
            elementToDrag.attachEvent("onlosecapture", upHandler);
        }
        else
        {
            return;
        }

        event.stopPropagation ? event.stopPropagation() : event.cancelBubble = true;
        event.preventDefault ? event.preventDefault() : event.returnValue = false;

        function moveHandler(e)
        {
            if (!e) e = window.event;

            elementToDrag.style.left = (e.clientX - deltaX) + "px";
            elementToDrag.style.top = (e.clientY - deltaY) + "px";

            e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
        }

        function upHandler(e)
        {
            if (!e) e = window.event;

            if (document.removeEventListener)
            {
                document.removeEventListener("mouseup", upHandler, true);
                document.removeEventListener("mousemove", moveHandler, true);
            }
            else if (document.detachEvent)
            {
                elementToDrag.detachEvent("onlosecapture", upHandler);
                elementToDrag.detachEvent("onmouseup", upHandler);
                elementToDrag.detachEvent("onmousemove", moveHandler);
                elementToDrag.releaseCapture();
            }
            else
            {
                return;
            }

            e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
        }
    },

    GetLocation: function(element)
    {
        var location = new Object();
        location.x = getX();
        location.y = getY();

        return location;

        function getX()
        {
            var x = 0;
            for (var e = element; e; e = e.offsetParent)
                x += e.offsetLeft;

            for (e = element; e && e != document.body; e = e.parentNode)
                if (e.scrollLeft) x -= e.scrollLeft;

            return x;
        }

        function getY()
        {
            var y = 0;
            for (var e = element; e; e = e.offsetParent)
                y += e.offsetTop;

            for (e = element; e && e != document.body; e = e.parentNode)
                if (e.scrollTop) y -= e.scrollTop;

            return y;
        }
    },

    SetFocus: function()
    {
        if (this.designMode && this.designModeInitialized)
            this.designModeFocusTarget.focus();
        else if (!this.designMode)
            this.htmlEditor.focus();
    },

    GetHtmlTextEditor: function()
    {
        return this.htmlModeDocument.getElementById('htmlTextEditor');
    },

    GetInnerHtml: function()
    {
        if (this.designMode)
            return this.designModeDocument.body.innerHTML;
        else
            return this.GetHtmlTextEditor().value;
    },

    RecordHtml: function()
    {
        this.recordedHtml = this.GetInnerHtml();
        this.set_modified(false);
    },

    MouseOver: function(ctrl)
    {
        if (this.isIE && this.browserVersion < 7) ctrl.style.padding = "0px";

        ctrl.style.border = "solid 1px " + this.get_buttonMouseOverBorderColor();
        ctrl.style.backgroundColor = this.get_buttonMouseOverColor();
        ctrl.style.cursor = 'pointer';
    },

    MouseOut: function(ctrl)
    {
        var bgColor = "transparent";

        if (this.isIE && this.browserVersion < 7)
        {
            ctrl.style.padding = "1px";
            ctrl.style.border = "none";
        }

        ctrl.style.borderColor = bgColor;
        ctrl.style.backgroundColor = bgColor;
    },

    SelectTab: function(designMode)
    {
        var designTab = $get(this.get_designTabID());
        var htmlTab = $get(this.get_htmlTabID());
        var selectedTab = designMode ? designTab : htmlTab;
        var unselectedTab = designMode ? htmlTab : designTab;

        selectedTab.style.fontWeight = "bold";
        unselectedTab.style.fontWeight = "normal";

        selectedTab.style.backgroundColor = this.get_selectedTabBackColor(); ;
        unselectedTab.style.backgroundColor = this.get_tabBackColor(); ;

        selectedTab.style.color = this.get_selectedTabTextColor(); ;
        unselectedTab.style.color = this.get_tabForeColor(); ;
    },

    IsTabSelected: function(ctrl)
    {
        var designTab = this.get_designTabID() == ctrl.id;
        var htmlTab = !designTab;

        if (designTab && this.designMode) return true;
        if (htmlTab && !this.designMode) return true;

        return false;
    },

    TabOver: function(ctrl)
    {
        if (!this.IsTabSelected(ctrl))
            ctrl.style.backgroundColor = this.get_tabMouseOverColor(); ;
    },

    TabOut: function(ctrl)
    {
        if (!this.IsTabSelected(ctrl))
            ctrl.style.backgroundColor = this.get_tabBackColor(); ;
    },

    SetTabLabel: function(tab, isModified, selected)
    {
        var e = tab.getElementsByTagName("span")[0];
        var text = e.innerHTML;
        var hasAsterick = text.endsWith('*');
        var showAsterick = this.get_showModifiedAsterick();
        var addAsterick = selected == true && isModified == true;
        var removeAsterick = selected == false || isModified == false;

        if (hasAsterick && removeAsterick) e.innerHTML = text.slice(0, -1);
        if (addAsterick && showAsterick && !hasAsterick) e.innerHTML = text + '*';
    },

    SetModifiedIndicators: function()
    {
        var isModified = this.get_modified();

        var designTab = $get(this.get_designTabID());
        var htmlTab = $get(this.get_htmlTabID());
        var selectedTab = this.designMode ? designTab : htmlTab;
        var unselectedTab = this.designMode ? htmlTab : designTab;

        this.SetTabLabel(selectedTab, isModified, true);
        this.SetTabLabel(unselectedTab, isModified, false);

        this.EnableToolbarElement("Button", "Save", isModified);
    },

    ClientEncode: function(html)
    {
        html = html.replace(/&quot;/g, '&quotx;');
        html = html.replace(/"/g, '&quot;');
        html = html.replace(/&amp;/g, '&ampx;');
        html = html.replace(/&/g, '&amp;');
        html = html.replace(/&lt;/g, '&ltx;');
        html = html.replace(/</g, '&lt;');
        html = html.replace(/&gt;/g, '&gtx;');
        html = html.replace(/>/g, '&gt;');

        return html;
    },

    ClientDecode: function(html)
    {
        html = html.replace(/&gt;/g, '>');
        html = html.replace(/&gtx;/g, '&gt;');
        html = html.replace(/&lt;/g, '<');
        html = html.replace(/&ltx;/g, '&lt;');
        html = html.replace(/&amp;/g, '&');
        html = html.replace(/&ampx;/g, '&amp;');
        html = html.replace(/&quot;/g, '"');
        html = html.replace(/&quotx;/g, '&quot;');

        return html;
    },

    HtmlEncode: function(html)
    {
        var div = document.createElement('div');
        var text = document.createTextNode(html);
        div.appendChild(text);

        return div.innerHTML;
    },

    ParseRule: function(rule)
    {
        var cc = rule.split(':');
        var conditionList = cc[0].split(',');
        var conversionList = cc[1].split(',');

        var r = new Object();
        r.conditions = new Object();
        r.conversion = new Object();

        for (var i = 0; i < conditionList.length; i++)
        {
            var pv = conditionList[i].split('=');
            switch (pv[0])
            {
                case ("unique"):
                    r.conditions.unique = pv[1];
                    break;
                case ("direction"):
                    r.conditions.direction = pv[1];
                    break;
                case ("browser"):
                    r.conditions.browser = pv[1];
                    break;
                case ("tag"):
                    r.conditions.tag = pv[1];
                    break;
                case ("attribute"):
                    r.conditions.attribute = pv[1];
                    break;
                case ("attributeValue"):
                    r.conditions.attributeValue = pv[1];
                    break;
                case ("style"):
                    r.conditions.style = pv[1];
                    break;
                case ("styleValue"):
                    r.conditions.styleValue = pv[1];
                    break;
                case ("text"):
                    r.conditions.text = pv[1];
                    break;
                case ("notext"):
                    r.conditions.notext = pv[1];
                    break;
                case ("noAttributes"):
                    r.conditions.noAttributes = pv[1];
                    break;
                case ("noChild"):
                    r.conditions.noChild = pv[1];
                    break;
                case ("hasChild"):
                    r.conditions.hasChild = pv[1];
                    break;
                case ("descendent"):
                    r.conditions.descendent = pv[1];
                    break;
                case ("parentInvalid"):
                    r.conditions.parentInvalid = pv[1];
                    break;
                case ("top"):
                    r.conditions.top = pv[1];
                    break;
                default:
                    break;
            }
        }

        for (i = 0; i < conversionList.length; i++)
        {
            pv = conversionList[i].split('=');
            switch (pv[0])
            {
                case ("tag"):
                    r.conversion.tag = pv[1];
                    break;
                case ("attribute"):
                    r.conversion.attribute = pv[1];
                    break;
                case ("attributeValue"):
                    r.conversion.attributeValue = pv[1];
                    break;
                case ("style"):
                    r.conversion.style = pv[1];
                    break;
                case ("styleValue"):
                    r.conversion.styleValue = pv[1];
                    break;
                case ("conversionType"):
                    r.conversion.conversionType = pv[1];
                    break;
                case ("text"):
                    r.conversion.text = pv[1];
                    break;
                case ("appendTag"):
                    r.conversion.appendTag = pv[1];
                    break;
                case ("appendIf"):
                    r.conversion.appendIf = pv[1];
                    break;
                case ("appendNewLine"):
                    r.conversion.appendNewLine = pv[1];
                    break;
                default:
                    break;
            }
        }

        return r;
    },

    InitializeRules: function()
    {
        var rules = [
        "direction=to,tag=font,attribute=size:tag=span,style=font-size,conversionType=fontsize",
        "direction=to,tag=font,attribute=face:tag=span,style=font-family",
        "direction=to,tag=font,attribute=color:tag=span,style=color",
        "direction=to,tag=font,style=background-color:tag=span",
        "direction=from,browser=ie,tag=span,style=font-size:tag=font,attribute=size,conversionType=fontsize",
        "direction=from,browser=ie,tag=span,style=font-family:tag=font,attribute=face",
        "direction=from,browser=ie,tag=span,style=color:tag=font,attribute=color",
        "direction=from,browser=ie,tag=span,style=background-color:tag=font",
        "direction=from,browser=o,tag=span,style=font-size:tag=font,attribute=size,conversionType=fontsize",
        "direction=from,browser=o,tag=span,style=font-family:tag=font,attribute=face",
        "direction=from,browser=o,tag=span,style=color:tag=font,attribute=color",
        "direction=from,browser=o,tag=span,style=background-color:tag=font",
        "direction=from,browser=ff,tag=span,style=font-size:tag=font,attribute=size,conversionType=fontsize",
        "direction=to,tag=u:tag=span,style=text-decoration,styleValue=underline",
        "unique=true,browser=ie,direction=from,tag=span,style=text-decoration,styleValue=underline:tag=u",
        "unique=true,browser=o,direction=from,tag=span,style=text-decoration,styleValue=underline:tag=u",
        "direction=to,tag=i:tag=span,style=text-style,styleValue=italic",
        "unique=true,browser=o,direction=from,tag=span,style=text-style,styleValue=italic:tag=i",
        "browser=ie,direction=to,tag=blockquote,style=margin-right,styleValue=0px:tag=div,style=margin-left,styleValue=40px",
        "browser=o,direction=to,tag=blockquote:tag=div,style=margin-left,styleValue=40px",
        "direction=to,tag=blockquote:tag=div",
        "browser=ie,direction=from,tag=div,style=margin-left,styleValue=40px:tag=blockquote,style=margin-right,styleValue=0px",
        "browser=o,direction=from,tag=div,style=margin-left,styleValue=40px:tag=blockquote",
        "unique=true,browser=ie,direction=to,tag=p,attribute=align:tag=p,style=text-align",
        "unique=true,browser=o,direction=to,tag=div,attribute=align:tag=div,style=text-align",
        "direction=to,attribute=align:style=text-align",
        "direction=to,attribute=valign:style=vertical-align",
        "unique=true,browser=ie,direction=from,tag=p,style=text-align:tag=p,attribute=align",
        "unique=true,browser=o,direction=from,tag=div,style=text-align:tag=div,attribute=align"
        ];

        var convertParagraphRules = [
        "browser=ie,direction=to,tag=p:tag=div",
        "browser=ie,direction=from,tag=div:tag=p",
        "browser=ie,direction=from,tag=div,style=margin-left,styleValue=40px:tag=blockquote,style=margin-right,styleValue=0px",
        "unique=true,browser=ie,direction=to,tag=p,noAttributes=true,notext=&nbsp;,hasChild=true,top=true:tag=none,appendTag=<br />,appendIf=notLast,appendNewLine=true",
        "unique=true,browser=ie,direction=to,tag=p,noAttributes=true,text=&nbsp;:tag=br",
        "unique=true,browser=ie,direction=to,tag=p,noAttributes=true,noChild=true:tag=none",
        "unique=true,browser=ie,direction=to,tag=hr,parentInvalid=true:tag=none",
        "browser=ie,direction=from,tag=br:tag=p,text=&nbsp;",
        "browser=ie,direction=from,tag=p:tag=p,appendTag=<p>&nbsp;</p>,appendIf=notNoAppend"
        ];

        this.conversionRules = new Array();

        for (var i = 0; i < rules.length; i++)
        {
            Array.add(this.conversionRules, this.ParseRule(rules[i]));
        }

        if (this.convertParagraphs)
        {
            for (i = 0; i < convertParagraphRules.length; i++)
            {
                Array.add(this.conversionRules, this.ParseRule(convertParagraphRules[i]));
            }
        }
    },

    InitializeLists: function()
    {
        this.noEndTags = "img,br,hr".split(',');
        this.blockTags = "p,div,blockquote,pre,br,hr,ul,ol,li,h1,h2,h3,h4,h5,h6,dl,dd,dt,address".split(',');
        this.listTags = "ul,ol,dl".split(',');
        this.fontSizes = "1:xx-small;2:x-small;3:small;4:medium;5:large;6:x-large;7:xx-large".split(';');
    },

    IsNoEndTag: function(tagName)
    {
        return Array.contains(this.noEndTags, tagName);
    },

    IsBlockTag: function(tagName)
    {
        return Array.contains(this.blockTags, tagName);
    },

    IsListTag: function(node)
    {
        return Array.contains(this.listTags, node.tagName.toLowerCase());
    },

    IsNoAppendTag: function(node)
    {
        if (node.nodeType != 1) return false;

        var tagName = node.tagName.toLowerCase();
        if (tagName == "p") return false;
        if (tagName == "br") return false;

        return this.IsBlockTag(tagName);
    },

    IsNoScriptAttribute: function(attributeName)
    {
        return Array.contains(this.noScriptAttributes, attributeName.toLowerCase());
    },

    IsAllowedTag: function(node)
    {
        return Array.contains(this.allowedTags, node.tagName.toLowerCase());
    },

    IsAllowedAttribute: function(attributeName)
    {
        return Array.contains(this.allowedAttributes, attributeName.toLowerCase());
    },

    IsTagName: function(element, tagName)
    {
        return element.tagName.toLowerCase() == tagName;
    },

    IsParentTag: function(node, name)
    {
        node = node.parentNode;

        while (node)
        {
            if (node.nodeName.toLowerCase() == name)
                return true;

            node = node.parentNode;
        }

        return false;
    },

    InPre: function(node, isTop)
    {
        var tagName = "pre";
        if (isTop) return false;
        if (this.IsTagName(node, tagName)) return true;

        return this.IsParentTag(node, tagName);
    },

    RemoveAttributeScript: function(attributeValue)
    {
        var value = attributeValue.toLowerCase().trim();
        var j = value.indexOf("javascript");
        if (j < 0) return attributeValue;
        if (value.substr(j).indexOf(':') < 0) return attributeValue;

        var valueParts = value.split(':');
        var len = valueParts.length - 1;
        for (var i = 0; i < len; i++)
        {
            if (valueParts[i].trim().endsWith("javascript"))
                return "javascript: void(0)";
        }

        return attributeValue;
    },

    FormatCssText: function(node)
    {
        var cssText = "";
        var rules = node.style.cssText.split(';');
        for (var i = 0; i < rules.length; i++)
        {
            var pv = rules[i].split(':')
            if (pv.length != 2) continue;

            if (cssText.length > 0) cssText += "; ";
            cssText += pv[0].trim().toLowerCase() + ": " + pv[1].trim();
        }

        return cssText;
    },

    LookUp: function(list, key, isKey)
    {
        var index = isKey ? 0 : 1;
        for (var i = 0; i < list.length; i++)
        {
            var kv = list[i].split(':');
            if (kv[index] == key) return kv[isKey ? 1 : 0];
        }

        return "";
    },

    RuleDelimiter: function(cssText)
    {
        return cssText.length > 0 ? "; " : "";
    },

    EncodeDoubleQuotes: function(s)
    {
        return s.replace(/"/g, '&#34;');
    },

    ReplaceEntityReferences: function(text)
    {
        text = text.replace(/\u0026/g, '&amp;');
        text = text.replace(/\u00A0/g, '&nbsp;');
        text = text.replace(/\u003C/g, '&lt;');
        text = text.replace(/\u003E/g, '&gt;');

        return text;
    },

    GetStyle: function(styles, index)
    {
        var pv = styles[index].split(':');
        if (pv.length != 2) return null;

        pv[0] = pv[0].trim();
        pv[1] = pv[1].trim();

        return pv;
    },

    FindStyle: function(cssText, property)
    {
        if (cssText.length == 0) return "";
        var styles = cssText.split(';');

        for (var i = 0; i < styles.length; i++)
        {
            var pv = this.GetStyle(styles, i);
            if (pv == null) continue;

            if (pv[0] == property) return pv[1];
        }

        return "";
    },

    HasStyle: function(cssText, property)
    {
        return this.FindStyle(cssText, property).length > 0 ? true : false;
    },

    AddStyle: function(cssText, addStyle)
    {
        if (addStyle.length == 0) return cssText;

        var pv = addStyle.split(':');
        if (this.HasStyle(cssText, pv[0])) return cssText;

        return cssText + this.RuleDelimiter(cssText) + addStyle;
    },

    AddStyles: function(cssText, list)
    {
        for (var i = 0; i < list.length; i++)
            cssText = this.AddStyle(cssText, list[i]);

        return cssText;
    },

    RemoveStyles: function(cssText, list)
    {
        var rules = cssText.split(';');
        var cssText = "";
        for (var i = 0; i < rules.length; i++)
        {
            var pv = rules[i].split(':')
            if (Array.contains(list, pv[0])) continue;

            if (cssText.length > 0) cssText += "; ";
            cssText += rules[i].trim();
        }

        return cssText;
    },

    AddAttributes: function(list)
    {
        var attributes = "";
        for (var i = 0; i < list.length; i++) attributes += " " + list[i];

        return attributes;
    },

    AttributeCount: function(attributes)
    {
        var count = 0;
        for (var i = 0; i < attributes.length; i++)
        {
            if (attributes[i].specified) count++;
        }

        return count;
    },

    FindAttribute: function(attributes, name)
    {
        for (var i = 0; i < attributes.length; i++)
        {
            var attribute = attributes[i];
            if (attribute.specified && attribute.name.toLowerCase() == name)
                return attribute;
        }

        return null;
    },

    ConvertValue: function(conversionType, value, isKey)
    {
        switch (conversionType)
        {
            case "fontsize":
                return this.LookUp(this.fontSizes, value, isKey);
                break;
            default:
                break;
        }

        return value;
    },

    IsEmptyParagraph: function(node)
    {
        if (node.nodeType != 1) return false;
        if (node.tagName.toLowerCase() != "p") return false;
        if (this.AttributeCount(node.attributes) != 0) return false;
        if (node.childNodes.length != 0) return false;

        return true;
    },

    IsDefaultParagraph: function(node)
    {
        if (node.nodeType != 1) return false;
        if (node.tagName.toLowerCase() != "p") return false;
        if (this.AttributeCount(node.attributes) != 0) return false;
        if (node.childNodes.length != 1) return false;
        if (node.childNodes[0].nodeType != 3) return false;

        var ev = this.HtmlEncode(node.childNodes[0].nodeValue);
        if (ev != "&nbsp;") return false;

        return true;
    },

    IsLast: function(node)
    {
        if (node.parentNode.lastChild == node) return true;
        if (this.isIE && node.nextSibling && node.parentNode.lastChild == node.nextSibling && this.IsDefaultParagraph(node.nextSibling)) return true;

        return false;
    },

    AddConversionItem: function(conversions, type, value)
    {
        if (!conversions)
            conversions = new Object();

        switch (type)
        {
            case ("changeTagName"):
                if (!conversions.changeTagName) conversions.changeTagName = new Object();
                conversions.changeTagName = value;
                break;
            case ("addAttribute"):
                if (!conversions.addAttributes) conversions.addAttributes = new Array();
                if (!Array.contains(conversions.addAttributes, value))
                    Array.add(conversions.addAttributes, value);
                break;
            case ("removeAttribute"):
                if (!conversions.removeAttributes) conversions.removeAttributes = new Array();
                if (!Array.contains(conversions.removeAttributes, value))
                    Array.add(conversions.removeAttributes, value);
                break;
            case ("addStyle"):
                if (!conversions.addStyles) conversions.addStyles = new Array();
                if (!Array.contains(conversions.addStyles, value))
                    Array.add(conversions.addStyles, value);
                break;
            case ("removeStyle"):
                if (!conversions.removeStyles) conversions.removeStyles = new Array();
                if (!Array.contains(conversions.removeStyles, value))
                    Array.add(conversions.removeStyles, value);
                break;
            case ("addText"):
                conversions.text = value;
                break;
            case ("appendTag"):
                conversions.appendTag = value;
                break;
            default:
                break;
        }

        return conversions;
    },

    AddConversionItems: function(conversions, rule, tagName, node, attributeValue, styleValue, toXHTML, inPre)
    {
        if (rule.conversion.tag && tagName != rule.conversion.tag)
            conversions = this.AddConversionItem(conversions, "changeTagName", rule.conversion.tag);

        if (rule.conversion.attribute && rule.conditions.style && styleValue.length > 0)
        {
            attributeValue = rule.conversion.attributeValue ? rule.conversion.attributeValue : styleValue;
            if (rule.conversion.conversionType) attributeValue = this.ConvertValue(rule.conversion.conversionType, attributeValue, false);

            conversions = this.AddConversionItem(conversions, "addAttribute", rule.conversion.attribute + '="' + this.EncodeDoubleQuotes(attributeValue) + '"');

            if (rule.conditions.attribute)
                conversions = this.AddConversionItem(conversions, "removeAttribute", rule.conditions.attribute);

            if (rule.conditions.style)
                conversions = this.AddConversionItem(conversions, "removeStyle", rule.conditions.style);
        }

        if (rule.conversion.style)
        {
            styleValue = rule.conversion.styleValue ? rule.conversion.styleValue : attributeValue;
            if (rule.conversion.conversionType) styleValue = this.ConvertValue(rule.conversion.conversionType, styleValue, true);

            conversions = this.AddConversionItem(conversions, "addStyle", rule.conversion.style + ": " + this.EncodeDoubleQuotes(styleValue));

            if (rule.conditions.style)
                conversions = this.AddConversionItem(conversions, "removeStyle", rule.conditions.style);

            if (rule.conditions.attribute)
                conversions = this.AddConversionItem(conversions, "removeAttribute", rule.conditions.attribute);
        }

        if (rule.conversion.text)
        {
            conversions = this.AddConversionItem(conversions, "addText", rule.conversion.text);
        }

        if (rule.conversion.appendTag)
        {
            var isLast = rule.conversion.appendIf && rule.conversion.appendIf == "notLast" && this.IsLast(node);
            var isNoAppend = rule.conversion.appendIf && rule.conversion.appendIf == "notNoAppend" && node.nextSibling && this.IsNoAppendTag(node.nextSibling);
            if (!isLast && !isNoAppend)
            {
                var appendTag = rule.conversion.appendTag;
                if (rule.conversion.appendNewLine && this.formatHtmlMode && toXHTML && !inPre) appendTag += this.newLine;

                conversions = this.AddConversionItem(conversions, "appendTag", appendTag);
            }
        }

        return conversions;
    },

    GetConversions: function(tagName, node, attributes, cssText, toXHTML, isTop, parentNode, inPre)
    {
        var conversions = null;

        var direction = toXHTML ? "to" : "from";
        var browser = this.isIE ? "ie" : this.isOpera ? "o" : this.isFirefox ? "ff" : "unknown";
        var attribute = null;
        var attributeValue = null;
        var style = null;
        var styleValue = null;
        var text = null;
        var unique = false;

        for (var i = 0; i < this.conversionRules.length; i++)
        {
            var rule = this.conversionRules[i];
            if (rule.conditions.tag && rule.conditions.tag != tagName) continue;
            if (rule.conditions.direction != direction) continue;
            if (rule.conditions.browser && rule.conditions.browser != browser) continue;
            if (rule.conditions.top && !isTop) continue;

            attributeValue = "";
            styleValue = "";
            text = "";
            unique = rule.conditions.unique && rule.conditions.unique == "true" ? true : false;

            if (rule.conditions.attribute)
            {
                attribute = this.FindAttribute(attributes, rule.conditions.attribute);
                if (!attribute) continue;

                attributeValue = attribute.value.trim();
                if (rule.conditions.attributeValue && rule.conditions.attributeValue != attributeValue.toLowerCase()) continue;
                if (unique && this.AttributeCount(attributes) != 1) continue;
                if (unique && cssText.length > 0) continue;
            }

            if (rule.conditions.style)
            {
                styleValue = this.FindStyle(cssText, rule.conditions.style).trim();
                if (styleValue.length == 0) continue;
                if (rule.conditions.styleValue && rule.conditions.styleValue != styleValue.toLowerCase()) continue;
                if (unique && cssText.split(';').length != 1) continue;
                if (unique && this.AttributeCount(attributes) != 1) continue;
            }

            if (rule.conditions.noAttributes)
            {
                if (this.AttributeCount(attributes) != 0) continue;
            }

            if (rule.conditions.noChild)
            {
                if (node.childNodes.length != 0) continue;
                var hasnochild = true;
            }

            if (rule.conditions.hasChild)
            {
                if (node.childNodes.length == 0) continue;
                var haschild = true;
            }

            if (rule.conditions.descendent)
            {
                if (!this.IsParentTag(node, "p")) continue;
                var descendent = true;
            }

            if (rule.conditions.parentInvalid)
            {
                if (node.parentNode == parentNode) continue;
                var parentInvalid = true;
            }

            if (rule.conditions.text)
            {
                if (node.childNodes.length != 1) continue;
                if (node.childNodes[0].nodeType != 3) continue;
                var ev = this.HtmlEncode(node.childNodes[0].nodeValue);
                if (ev != rule.conditions.text) continue;
            }

            if (rule.conditions.notext)
            {
                if (node.childNodes.length == 1 &&
                    node.childNodes[0].nodeType == 3)
                {
                    var ev = this.HtmlEncode(node.childNodes[0].nodeValue);
                    if (ev == rule.conditions.notext) continue;
                }
            }

            conversions = this.AddConversionItems(conversions, rule, tagName, node, attributeValue, styleValue, toXHTML, inPre);
        }

        return conversions;
    },

    CreateParagraphInfo: function()
    {
        var paragraphInfo = new Object();
        paragraphInfo.inParagraph = false;
        paragraphInfo.skipNode = false;

        return paragraphInfo;
    },

    InsertParagraphHTML: function(paragraphInfo, isElement, node)
    {
        var html = "";
        var tagName = isElement ? node.tagName.toLowerCase() : null;
        var isBlock = isElement && this.IsBlockTag(tagName);
        paragraphInfo.skipNode = false;

        if (!paragraphInfo.inParagraph && !isBlock)
        {
            html += "<p>";
            paragraphInfo.inParagraph = true;
        }
        else if (paragraphInfo.inParagraph && isBlock)
        {
            html += this.EndParagraphHTML(paragraphInfo);
            paragraphInfo.skipNode = tagName == "br";
        }
        else if (isBlock && !paragraphInfo.inParagraph)
        {
            if (tagName == "br")
            {
                html += "<p>&nbsp;</p>";
                paragraphInfo.skipNode = true;
            }
        }

        return html;
    },

    EndParagraphHTML: function(paragraphInfo)
    {
        paragraphInfo.inParagraph = false;
        return "</p>";
    },

    WriteHTMLAttributes: function(attributes, cssText, conversions)
    {
        var html = "";

        for (var i = 0; i < attributes.length; i++)
        {
            var attribute = attributes[i];
            if (attribute.specified)
            {
                var name = attribute.name.toLowerCase();
                if (!this.IsAllowedAttribute(name)) continue;
                if (name == 'style') continue;
                if (conversions && conversions.removeAttributes && Array.contains(conversions.removeAttributes, name)) continue;

                var value = decodeURIComponent(attribute.value);
                if (this.IsNoScriptAttribute(name)) value = this.RemoveAttributeScript(value);

                html += " " + name + '="' + this.EncodeDoubleQuotes(value) + '"';
            }
        }

        if (conversions)
        {
            if (conversions.addStyles)
                cssText = this.AddStyles(cssText, conversions.addStyles);

            if (conversions.removeStyles)
                cssText = this.RemoveStyles(cssText, conversions.removeStyles);

            if (conversions.addAttributes)
                html += this.AddAttributes(conversions.addAttributes);
        }

        if (cssText.length > 0)
        {
            html += ' style="' + this.EncodeDoubleQuotes(cssText) + '"';
        }

        return html;
    },

    WriteHTMLTag: function(node, toXHTML, isTop, parentNode, inPre)
    {
        var tagName = node.tagName.toLowerCase();
        var isPre = tagName == "pre";
        if (inPre && isPre)
            return this.WriteInnerHTML(node.childNodes, toXHTML, false, node, inPre);

        var attributes = node.attributes;
        var cssText = this.FormatCssText(node);
        var html = "";

        var conversions = this.convertDeprecatedSyntax ? this.GetConversions(tagName, node, attributes, cssText, toXHTML, isTop, parentNode, inPre) : null;
        if (conversions && conversions.changeTagName) tagName = conversions.changeTagName;
        if (conversions && conversions.changeTagName && tagName == "none")
        {
            html += this.WriteInnerHTML(node.childNodes, toXHTML, false, node, inPre);

            if (conversions.appendTag)
                html += conversions.appendTag;

            return html;
        }

        html += "<" + tagName;

        html += this.WriteHTMLAttributes(attributes, cssText, conversions);

        if (this.IsNoEndTag(tagName))
        {
            html += " />";
        }
        else
        {
            var text = conversions && conversions.text ? conversions.text : this.WriteInnerHTML(node.childNodes, toXHTML, false, node, inPre);
            var formatBreak = this.formatHtmlMode && toXHTML && this.IsListTag(node) && !inPre ? this.newLine : "";
            html += ">" + formatBreak + text + "</" + tagName + ">";
        }

        if (this.formatHtmlMode && toXHTML && this.IsBlockTag(tagName) && !inPre) html += this.newLine;

        if (conversions && conversions.appendTag)
            html += conversions.appendTag;

        return html;
    },

    WriteTextNode: function(node, toXHTML, inPre)
    {
        var text = node.nodeValue;

        if (inPre)
        {
            text = this.ReplaceEntityReferences(text);
        }
        else
        {
            text = this.HtmlEncode(text);
            if (this.replaceNoBreakSpace && toXHTML) text = text.replace(/&nbsp;/, " ");
        }

        return text;
    },

    WriteInnerHTML: function(childNodes, toXHTML, isTop, parentNode, inPre)
    {
        var html = "";
        inPre = inPre ? true : this.InPre(parentNode, isTop);
        var paragraphInfo = this.convertParagraphs && isTop && !toXHTML ? this.CreateParagraphInfo() : null;

        for (var i = 0; i < childNodes.length; i++)
        {
            var node = childNodes[i];
            if (node.parentNode != parentNode) continue;
            if (this.IsEmptyParagraph(node)) continue;
            var isElement = node.nodeType == 1 && this.IsAllowedTag(node);
            var isText = node.nodeType == 3;
            if (!isElement && !isText) continue;

            if (paragraphInfo)
            {
                html += this.InsertParagraphHTML(paragraphInfo, isElement, node);
                if (paragraphInfo.skipNode) continue;
            }

            if (isElement)
            {
                html += this.WriteHTMLTag(node, toXHTML, isTop, parentNode, inPre);
            }
            else if (isText)
            {
                html += this.WriteTextNode(node, toXHTML, inPre);
            }
        }

        if (paragraphInfo && paragraphInfo.inParagraph)
        {
            html += this.EndParagraphHTML(paragraphInfo);
        }

        return html;
    },

    ReplaceDefaultTags: function(html, toXHTML)
    {
        if (this.formatHtmlMode)
        {
            while (html.endsWith(this.newLine))
                html = html.slice(0, -this.newLine.length);
        }

        var defaultTags = this.isIE ? "<p>&nbsp;</p>" : "<br />";
        var hasDefaultTags = html.endsWith(defaultTags);

        if (!toXHTML)
        {
            if (!hasDefaultTags && this.isFirefox) html += defaultTags;
        }
        else
        {
            if (hasDefaultTags) html = html.slice(0, -defaultTags.length);
        }

        return html;
    },

    ConvertHTML: function(html, toXHTML)
    {
        var div = document.createElement('div');
        div.innerHTML = html;

        html = this.WriteInnerHTML(div.childNodes, toXHTML, true, div, false);
        html = this.ReplaceDefaultTags(html, toXHTML);

        return html;
    },

    GetDecodedHtml: function()
    {
        var htmlencodedText = $get(this.get_htmlencodedTextID());
        return this.ClientDecode(htmlencodedText.value);
    },

    CheckModified: function()
    {
        var html = this.GetInnerHtml();
        if (html != this.recordedHtml) this.set_modified(true);
    },

    StoreHtml: function(html)
    {
        this.innerHTML = html;
        if (this._modified == false && html != this.recordedHtml) this.set_modified(true);
    },

    CopyHtmlToDesign: function()
    {
        var html = this.innerHTML;
        if (this.outputXHTML) html = this.ConvertHTML(html, false);
        if (html.length == 0 && this.isFirefox) html = "<br />";

        this.designModeDocument.body.innerHTML = html;
    },

    CopyDesignToHtml: function()
    {
        this.StoreHtml(this.designModeDocument.body.innerHTML);
    },

    CopyHtmlEditorToHtml: function()
    {
        this.StoreHtml(this.GetHtmlTextEditor().value);
    },

    CopyHtmlToHtmlEditor: function()
    {
        this.GetHtmlTextEditor().value = this.outputXHTML ? this.ConvertHTML(this.innerHTML, true) : this.innerHTML;
    },

    SetCursor: function(wait, element)
    {
        var e = wait ? element : this.waitElement;
        if (e) e.style.cursor = wait ? "wait" : "pointer";

        this.waitElement = wait ? element : null;
    },

    DisplayWait: function(wait, element)
    {
        this.SetCursor(wait, element);
    },

    MatchToolbarElement: function(toolbarElement, type, name)
    {
        return toolbarElement.type == type && toolbarElement.name == name;
    },

    FindToolbarElement: function(type, name)
    {
        for (var i = 0; i < this.toolbarInfo.toolbars.length; i++)
        {
            var toolbarInfo = this.toolbarInfo.toolbars[i];
            if (this.MatchToolbarElement(toolbarInfo, type, name))
                return toolbarInfo;

            for (var ii = 0; ii < toolbarInfo.items.length; ii++)
            {
                var toolbarInfoItem = toolbarInfo.items[ii];
                if (this.MatchToolbarElement(toolbarInfoItem, type, name))
                    return toolbarInfoItem;

                if (toolbarInfoItem.type != "Buttons") continue;

                for (var iii = 0; iii < toolbarInfoItem.buttons.length; iii++)
                {
                    var buttonInfo = toolbarInfoItem.buttons[iii];
                    if (this.MatchToolbarElement(buttonInfo, type, name))
                        return buttonInfo;
                }
            }
        }

        return null;
    },

    EnableToolbarElement: function(type, name, enabled)
    {
        var toolbarElement = this.FindToolbarElement(type, name);
        if (!toolbarElement) return;

        this.EnableElement(toolbarElement, enabled);
    },

    IsModeEnabled: function(mode, enabled)
    {
        if (enabled == "Always")
            return true;
        else if (enabled == "Never")
            return false;
        else
            return enabled == mode;
    },

    IsDependencyEnabled: function(mode, dependency)
    {
        if (dependency == "None") return true;

        if (dependency == "Editable")
        {
            if (mode == "Design")
                return this.get_designModeEditable();
            else if (mode == "Html")
                return this.get_htmlModeEditable();
        }

        return false;
    },

    IsToolbarElementEnabled: function(mode, toolbarElement)
    {
        if (!this.IsModeEnabled(mode, toolbarElement.enabled))
            return false;

        return this.IsDependencyEnabled(mode, toolbarElement.dependency);
    },

    EnableElement: function(toolbarElement, enabled)
    {
        var element = $get(toolbarElement.id);
        if (!element) return;

        if (toolbarElement.disableMethod == "Opacity")
        {
            var hidePercent = "40";
            var opacity = enabled ? "1.0" : "." + hidePercent;
            var opacityIE = "alpha(opacity=" + (enabled ? "100" : hidePercent) + ")";

            if (this.isIE)
                element.style.filter = opacityIE;
            else
                element.style.opacity = opacity;
        }
        else if (toolbarElement.disableMethod == "Hide")
        {
            if (enabled)
            {
                var display = "inline";

                switch (toolbarElement.type)
                {
                    case "Toolbar":
                        display = "block";
                        break;
                    case "Select":
                    case "Buttons":
                    case "Button":
                        display = "inline"
                        break;
                    default:
                        break;
                }

                element.style.display = display;
            }
            else
            {
                element.style.display = "none";
            }
        }

        element.disabled = enabled ? false : true;
    },

    EnableToolbarButtons: function(buttons, mode, enable)
    {
        for (var i = 0; i < buttons.length; i++)
        {
            var buttonInfo = buttons[i];
            var enabled = enable && this.IsToolbarElementEnabled(mode, buttonInfo);

            this.EnableElement(buttonInfo, enabled);
        }
    },

    EnableToolbars: function(mode)
    {
        for (var i = 0; i < this.toolbarInfo.toolbars.length; i++)
        {
            var toolbarInfo = this.toolbarInfo.toolbars[i];
            var toolbarEnabled = this.IsToolbarElementEnabled(mode, toolbarInfo);

            for (var ii = 0; ii < toolbarInfo.items.length; ii++)
            {
                var toolbarInfoItem = toolbarInfo.items[ii];
                var toolbarInfoItemEnabled = toolbarEnabled && this.IsToolbarElementEnabled(mode, toolbarInfoItem);

                switch (toolbarInfoItem.type)
                {
                    case "Select":
                        this.EnableElement(toolbarInfoItem, toolbarInfoItemEnabled && this.IsToolbarElementEnabled(mode, toolbarInfoItem));
                        break;
                    case "Buttons":
                        this.EnableToolbarButtons(toolbarInfoItem.buttons, mode, toolbarInfoItemEnabled);
                        break;
                    default:
                        continue;
                }
            }
        }
    },

    EmptySelection: function()
    {
        if (!this.isIE) return;

        if (this.designMode && this.designModeInitialized)
            this.designModeDocument.selection.empty();
        else if (!this.designMode)
            this.htmlModeDocument.selection.empty();
    },

    SwitchMode: function(element, mode)
    {
        if (mode == "Design" && this.designMode) return;
        if (mode == "Html" && !this.designMode) return;

        var f = null;

        switch (mode)
        {
            case "Html":
                f = this.GoToHtmlMode;
                break;
            case "Design":
                f = this.GoToDesignMode;
                break;
            default:
                return;
        }

        this.DisplayWait(true, element);
        this.TimeoutInvoke(f);
    },

    GoToHtmlMode: function()
    {
        if (!this.designMode) return;

        this.CopyDesignToHtml();

        var isModified = this._modified;
        this.CopyHtmlToHtmlEditor();

        if (this.isIE)
            this.EmptySelection()

        var disableDesignMode = this.isIE || this.isOpera ? false : this.designModeInitialized;

        if (disableDesignMode)
            this.SetDesignMode(false);

        this.ShowHtmlEditor(true);
        this.ShowDesignEditor(false);
        this.SelectTab(false);
        this.EnableToolbars("Html");
        this.DisplayWait(false);

        this.designMode = false;

        if (!isModified) this.RecordHtml();
        this.SetModifiedIndicators();
    },

    GoToDesignMode: function()
    {
        if (this.designMode) return;

        this.CopyHtmlEditorToHtml();

        var isModified = this._modified;
        this.CopyHtmlToDesign();

        this.EmptySelection();

        this.ShowDesignEditor(true);
        this.ShowHtmlEditor(false);
        this.SelectTab(true);
        this.EnableToolbars("Design");
        this.DisplayWait(false);

        var restoreDesignMode = this.isIE || this.isOpera ? false : this.designModeInitialized;

        if (restoreDesignMode)
        {
            if (this.SetDesignMode(true))
                this.SetCSSMode();
        }

        this.designMode = true;

        if (!isModified) this.RecordHtml();
        this.SetModifiedIndicators();

        this.TimeoutInvoke(this.RaiseContextChanged);
    },

    ShowDesignEditor: function(show)
    {
        this.ShowElement(this.designArea, show, false);
    },

    ShowHtmlEditor: function(show)
    {
        this.ShowElement(this.htmlArea, show, false);
    },

    ShowElement: function(e, show, inline)
    {
        e.style.display = show == false ? "none" : inline ? "inline" : "block";
    },

    TimeoutInvoke: function(f, ms)
    {
        f = Function.createDelegate(this, f);
        window.setTimeout(f, arguments.length > 1 ? ms : 100);
    },

    InitializeHTML: function()
    {
        this.outputXHTML = this.get_outputXHTML();
        this.convertDeprecatedSyntax = this.outputXHTML && this.get_convertDeprecatedSyntax();
        this.convertParagraphs = this.isIE && this.convertDeprecatedSyntax && this.get_convertParagraphs();
        this.replaceNoBreakSpace = this.isIE && this.outputXHTML && this.get_replaceNoBreakSpace();
        this.allowedTags = this.get_allowedTags().split(',');
        this.allowedAttributes = this.get_allowedAttributes().split(',');
        this.noScriptAttributes = this.get_noScriptAttributes().split(',');
        this.formatHtmlMode = this.isIE && this.get_formatHtmlMode() && !this.standardsMode;
        this.newLine = this.get_newLine();

        this.InitializeLists();
        this.InitializeRules();

        var html = this.ClientDecode($get(this.get_htmlencodedTextID()).value);
        html = this.outputXHTML ? this.ConvertHTML(html, !this.designMode) : html;

        this.StoreHtml(html);
        this.SetValue(html, this.designMode);
    },

    GetInitialHtmlEditorOuterHTML: function()
    {
        var designFrame = $get(this.get_designEditorID());
        var h = parseInt(designFrame.height) - 20;
        var w = parseInt(designFrame.width) - 20;
        var bgColor = "background-color:" + this.get_editorBackColor();
        var fgColor = "color:" + this.get_editorForeColor();
        var readOnly = this.get_htmlModeEditable() ? "" : " readonly='true'";

        var outerHTML =
            "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
            "<html xmlns='http://www.w3.org/1999/xhtml'>" +
            "<head>" +
            "<style type='text/css'>" +
            "body {margin:0; padding:0; " + bgColor + "; " + fgColor + ";}" +
            "</style>" +
            "</head>" +
            "<body>" +
            "<form>" +
            "<textarea id='htmlTextEditor' style='overflow:auto; font-family:Tahoma; resize:none; font-size:10pt; border:0; margin:0; position: absolute; right: 10px; top:10px; width:" + w + "px; " + "height:" + h + "px; " + bgColor + "; " + fgColor + ";' rows='' cols='' " + readOnly + "></textarea>" +
            "</form>" +
            "</body>" +
            "</html>";

        return outerHTML;
    },

    GetInitialEditorInnerHTML: function()
    {
        var html = this.innerHTML;
        if (html.length == 0 && this.isFirefox) html = "<br />";

        return html;
    },

    SetInitialEditor: function()
    {
        if (this.designMode)
        {
            this.designModeDocument.body.innerHTML = this.GetInitialEditorInnerHTML();
            this.EnableToolbars("Design");
        }
        else
        {
            this.GetHtmlTextEditor().value = this.innerHTML;
            this.EnableToolbars("Html");
        }
    },

    GetInitialDesignEditorOuterHTML: function()
    {
        var bgColor = "background-color:" + this.get_editorBackColor();
        var fgColor = "color:" + this.get_editorForeColor();
        var dir = this.get_textDirection() == "RightToLeft" ? " dir='rtl'" : "";
        var pStyle = this.convertParagraphs ? "p {margin:0;}" : "";
        var metaIE7 = this.isIE && this.get_designModeEmulateIE7() ? "<meta http-equiv='X-UA-Compatible' content='IE=EmulateIE7' />" : "";

        var outerHTML =
            "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
            "<html xmlns='http://www.w3.org/1999/xhtml'" + dir + ">" +
            "<head>" +
            "<title>Design Editor Frame</title>" +
            metaIE7 +
            "<style type='text/css'>" +
            "body {" + bgColor + "; " + fgColor + ";}" + pStyle +
            "</style>" +
            ((this.get_designModeCss().length > 0) ? "<link rel='stylesheet' href='" + this.get_designModeCss() + "' type='text/css' />" : "") +
            "</head>" +
            "<body></body>" +
            "</html>";

        return outerHTML;
    },

    SetCSSMode: function()
    {
        if (!this.get_designModeEditable()) return false;
        if (!this.designModeInitialized) return false;

        try
        {
            this.designModeDocument.execCommand(this.cssCommand, false, true);
        }
        catch (e)
        {
            return false;
        }

        return true;
    },

    SetDesignMode: function(on)
    {
        if (!this.get_designModeEditable()) return false;

        var setting = on ? "On" : "Off";

        try
        {
            this.designModeDocument.designMode = setting;
            if (this.isIE)
            {
                this.designModeDocument = this.designEditor.contentWindow.document;
                this.designModeFocusTarget = this.designModeDocument.body;
            }
            else if (this.isFirefox && on)
            {
                this.designModeDocument.designMode = "Off";
                this.designModeDocument.designMode = setting;
            }

            this.designModeInitialized = true;
        }
        catch (e)
        {
            this.designModeInitialized = false;
        }

        return this.designModeInitialized;
    },

    InitializeDesignMode: function()
    {
        if (!this.SetDesignMode(true)) return;
        if (this.isIE) return;

        this.cssCommand = "useCSS";
        if (this.isFirefox && this.IsCommandSupported("styleWithCSS"))
            this.cssCommand = "styleWithCSS";

        this.SetCSSMode();
    },

    InitializeValidators: function()
    {
        var validatorIDs = this.get_validatorIDs();
        if (validatorIDs == null) return;
        if (validatorIDs.length == 0) return;

        var validators = Page_Validators;

        var index = Array.indexOf(validators, $get(validatorIDs[0]));
        if (index == 0) return;

        for (var i = validatorIDs.length - 1; i >= 0; i--)
        {
            var validator = $get(validatorIDs[i]);

            Array.remove(validators, validator);
            Array.insert(validators, 0, validator);
        }
    },

    InitializeDialogInfo: function()
    {
        this.dialogInfo = new Object();
        this.dialogInfo.elementIDs = new Object();
        this.dialogInfo.dialogs = new Array();
        this.dialogInfo.options = new Object();
        this.dialogInfo.current = null;

        var elementIDs = this.get_dialogBoxIDs().split(';');

        this.dialogInfo.elementIDs.dialogShim = elementIDs[0];
        this.dialogInfo.elementIDs.dialogBox = elementIDs[1];
        this.dialogInfo.elementIDs.dialogHeading = elementIDs[2];
        this.dialogInfo.elementIDs.dialogFrame = elementIDs[3];

        var data = this.get_dialogData();
        if (data.length == 0) return;

        var dialogs = data.split(';');

        for (var i = 0; i < dialogs.length; i++)
        {
            this.dialogInfo.dialogs[i] = new Object();

            var dialogItems = dialogs[i].split(',');

            this.dialogInfo.dialogs[i].name = dialogItems[0];
            this.dialogInfo.dialogs[i].frameUrl = dialogItems[1];
            this.dialogInfo.dialogs[i].heading = dialogItems[2];
            this.dialogInfo.dialogs[i].height = dialogItems[3];
            this.dialogInfo.dialogs[i].width = dialogItems[4];
        }

        var dialogDimensions = this.get_dialogDimensions().split(';');
        this.dialogInfo.options.dimensions = new Object();
        this.dialogInfo.options.dimensions.headingHeight = dialogDimensions[0];
        this.dialogInfo.options.dimensions.buttonbarHeight = dialogDimensions[1];

        var dialogColors = this.get_dialogColors().split(';');
        this.dialogInfo.options.colors = new Object();
        this.dialogInfo.options.colors.body = dialogColors[0];
        this.dialogInfo.options.colors.text = dialogColors[1];
        this.dialogInfo.options.colors.heading = dialogColors[2];
        this.dialogInfo.options.colors.headingText = dialogColors[3];
        this.dialogInfo.options.colors.buttonBar = dialogColors[4];
        this.dialogInfo.options.colors.borders = dialogColors[5];
        this.dialogInfo.options.colors.table = dialogColors[6];
        this.dialogInfo.options.colors.tabText = dialogColors[7];
        this.dialogInfo.options.colors.selectedTabText = dialogColors[8];
        this.dialogInfo.options.colors.selectedTab = dialogColors[9];
        this.dialogInfo.options.colors.unselectedTab = dialogColors[10];

        this.dialogInfo.options.pixelImageUrl = this.get_pixelImageUrl();
        this.dialogInfo.options.colors.useColorNames = true;
    },

    InitializeToolbarInfo: function()
    {
        var toolbarData = this.get_toolbarData();
        var toolbars = toolbarData.length == 0 ? [] : toolbarData.split(';');
        var o = new Object();
        o.toolbars = new Array();

        for (var i = 0; i < toolbars.length; i++)
        {
            o.toolbars[i] = new Object();
            o.toolbars[i].type = 'Toolbar';
            var toolbarInfo = toolbars[i].split('$');
            var toolbarNameParts = toolbarInfo[0].split('@');
            o.toolbars[i].name = toolbarNameParts[0];
            var toolbarAttributes = toolbarNameParts[1].split('#');
            o.toolbars[i].enabled = toolbarAttributes[0];
            o.toolbars[i].dependency = toolbarAttributes[1];
            o.toolbars[i].disableMethod = toolbarAttributes[2];
            o.toolbars[i].id = toolbarAttributes[3];
            o.toolbars[i].items = new Array();

            var toolbarInfoItems = toolbarInfo[1].split('|');
            for (var ii = 0; ii < toolbarInfoItems.length; ii++)
            {
                o.toolbars[i].items[ii] = new Object();
                var toolbarInfoItem = toolbarInfoItems[ii].split('=');
                var toolbarInfoItemParts = toolbarInfoItem[0].split('?');
                o.toolbars[i].items[ii].type = toolbarInfoItemParts[0];
                var toolbarInfoItemNameParts = toolbarInfoItemParts[1].split('@');
                o.toolbars[i].items[ii].name = toolbarInfoItemNameParts[0];
                var toolbarInfoItemAttributes = toolbarInfoItemNameParts[1].split('#');
                o.toolbars[i].items[ii].enabled = toolbarInfoItemAttributes[0];
                o.toolbars[i].items[ii].dependency = toolbarInfoItemAttributes[1];
                o.toolbars[i].items[ii].disableMethod = toolbarInfoItemAttributes[2];
                o.toolbars[i].items[ii].id = toolbarInfoItemAttributes[3];

                if (o.toolbars[i].items[ii].type == "Buttons")
                {
                    o.toolbars[i].items[ii].buttons = new Array();
                    var buttons = toolbarInfoItem.length == 1 ? [] : toolbarInfoItem[1].split(',');
                    for (var iii = 0; iii < buttons.length; iii++)
                    {
                        o.toolbars[i].items[ii].buttons[iii] = new Object();
                        o.toolbars[i].items[ii].buttons[iii].type = 'Button';
                        var buttonNameParts = buttons[iii].split('@');
                        o.toolbars[i].items[ii].buttons[iii].name = buttonNameParts[0];
                        var buttonAttributes = buttonNameParts[1].split('#');
                        o.toolbars[i].items[ii].buttons[iii].enabled = buttonAttributes[0];
                        o.toolbars[i].items[ii].buttons[iii].dependency = buttonAttributes[1];
                        o.toolbars[i].items[ii].buttons[iii].disableMethod = buttonAttributes[2];
                        o.toolbars[i].items[ii].buttons[iii].id = buttonAttributes[3];
                    }
                }
            }
        }

        this.toolbarInfo = o;
    },

    IsStandardsMode: function()
    {
        try
        {
            var l = arguments.caller.length; // standards mode issues script error
            return false;
        }
        catch (e)
        {
            return true;
        }
    },

    InitializeEditor: function()
    {
        this.isIE = Sys.Browser.agent == Sys.Browser.InternetExplorer;
        this.isFirefox = Sys.Browser.agent == Sys.Browser.Firefox;
        this.isOpera = Sys.Browser.agent == Sys.Browser.Opera;
        this.browserVersion = Sys.Browser.version;
        this.standardsMode = this.IsStandardsMode();

        this.designMode = this.get_initialMode() == "Design" ? true : false;
        this.htmlEditor = this.isIE ? $get(this.get_htmlEditorID()) :
                                      $get(this.get_htmlEditorID()).contentWindow;
        this.designEditor = this.isIE ? $get(this.get_designEditorID()) :
                                        $get(this.get_designEditorID()).contentWindow;

        this.htmlArea = $get(this.get_htmlAreaID());
        this.designArea = $get(this.get_designAreaID());

        if (this.isIE)
        {
            this.htmlModeDocument = this.htmlEditor.contentWindow.document;
            this.designModeDocument = this.designEditor.contentWindow.document;
        }
        else
        {
            this.htmlModeDocument = this.htmlEditor.document;
            this.designModeDocument = this.designEditor.document;
        }

        this.InitializeValidators();
        this.InitializeDialogInfo();
        this.InitializeToolbarInfo();
        this.InitializeHTML();

        this.designModeDocument.open("text/html", "replace");
        this.designModeDocument.write(this.GetInitialDesignEditorOuterHTML());
        this.designModeDocument.close();

        this.htmlModeDocument.open("text/html", "replace");
        this.htmlModeDocument.write(this.GetInitialHtmlEditorOuterHTML());
        this.htmlModeDocument.close();

        this.InitializeDesignMode();

        this.PostDesignModeDocumentCreate();

        this.endRequestHandler = Function.createDelegate(this, this.OnUpdateHtml);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(this.endRequestHandler);
    },

    DisposeEditor: function()
    {
        Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(this.endRequestHandler);
        this.endRequestHandler = null;

        this.ClearEventHandlers();
    },

    initialize: function()
    {
        ParsElecom.HtmlEditor.HtmlEditor.callBaseMethod(this, 'initialize');

        this.TimeoutInvoke(this.InitializeEditor, 0);
        this.initialized = true;
    },

    dispose: function()
    {
        this.StopTimer();
        this.DisposeEditor();

        ParsElecom.HtmlEditor.HtmlEditor.callBaseMethod(this, 'dispose');
    },

    add_modifiedChanged: function(handler)
    {
        this.get_events().addHandler('modifiedChanged', handler);
    },

    remove_modifiedChanged: function(handler)
    {
        this.get_events().removeHandler('modifiedChanged', handler);
    },

    add_contextChanged: function(handler)
    {
        this.get_events().addHandler('contextChanged', handler);
    },

    remove_contextChanged: function(handler)
    {
        this.get_events().removeHandler('contextChanged', handler);
    },

    RaiseModifiedChanged: function()
    {
        var handler = this.get_events().getHandler('modifiedChanged');
        if (handler)
        {
            var isModified = this.get_modified();
            handler(this, isModified);
        }
    },

    RaiseContextChanged: function()
    {
        var handler = this.get_events().getHandler('contextChanged');
        if (handler)
        {
            var context = this.designMode ? this.GetDesignModeContext() : null;
            handler(this, context);
        }
    },

    PostDesignModeDocumentCreate: function()
    {
        this.designModeInitializedCount = 0;
        this.TimeoutInvoke(this.OnDesignModeDocumentCreate);
    },

    OnDesignModeDocumentCreate: function()
    {
        if (this.designModeDocument.body == null && this.designModeInitializedCount < 100)
        {
            this.designModeInitializedCount++;
            this.TimeoutInvoke(this.OnDesignModeDocumentCreate);

            return;
        }

        this.designModeFocusTarget = this.isIE ? this.designModeDocument.body :
                                                 this.designEditor;

        this.SetInitialEditor();
        this.RecordHtml();
        this.InitializeEventHandlers();
        this.TimeoutInvoke(this.RaiseContextChanged);
    },

    InitializeEventHandlers: function()
    {
        this.designModeFocusElement = this.isIE ? this.designEditor : this.designModeDocument;
        this.htmlModeFocusElement = this.isIE ? this.htmlEditor : this.htmlModeDocument;
        this.dialogFrameElement = $get(this.dialogInfo.elementIDs.dialogFrame);

        this.onDesignEditorBlurHandler = Function.createDelegate(this, this.StoreDesignEditor);
        this.onHtmlEditorBlurHandler = Function.createDelegate(this, this.StoreHtmlEditor);
        this.onDialogFrameLoadHandler = Function.createDelegate(this, this.OnDialogFrameLoad);

        $addHandler(this.designModeFocusElement, 'blur', this.onDesignEditorBlurHandler);
        $addHandler(this.htmlModeFocusElement, 'blur', this.onHtmlEditorBlurHandler);

        this.onEditorFocusHandler = Function.createDelegate(this, this.OnEditorFocus);
        $addHandler(this.designModeFocusElement, 'focus', this.onEditorFocusHandler);
        $addHandler(this.htmlModeFocusElement, 'focus', this.onEditorFocusHandler);

        $addHandler(this.dialogFrameElement, "load", this.onDialogFrameLoadHandler);

        if (this.isIE)
        {
            this.onDesignEditorKeydownHandler = Function.createDelegate(this, this.OnDesignEditorKeyEvent);
            this.designModeDocument.body.attachEvent("onkeydown", this.onDesignEditorKeydownHandler);

            this.onDesignEditorMousedownHandler = Function.createDelegate(this, this.OnDesignEditorMouseEvent);
            this.designModeDocument.body.attachEvent("onmousedown", this.onDesignEditorMousedownHandler);
        }
        else
        {
            this.onDesignEditorKeypressHandler = Function.createDelegate(this, this.OnDesignEditorKeyEvent);
            $addHandler(this.designModeFocusElement, 'keypress', this.onDesignEditorKeypressHandler);

            this.onDesignEditorMousedownHandler = Function.createDelegate(this, this.OnDesignEditorMouseEvent);
            $addHandler(this.designModeFocusElement, 'mousedown', this.onDesignEditorMousedownHandler);
        }
    },

    ClearEventHandlers: function()
    {
        if (this.isIE)
        {
            this.designModeDocument.body.detachEvent('onmousedown', this.onDesignEditorMousedownHandler);
            this.onDesignEditorMousedownHandler = null;

            this.designModeDocument.body.detachEvent('onkeydown', this.onDesignEditorKeydownHandler);
            this.onDesignEditorKeydownHandler = null;
        }
        else
        {
            $removeHandler(this.designModeFocusElement, 'mousedown', this.onDesignEditorMousedownHandler);
            this.onDesignEditorMousedownHandler = null;

            $removeHandler(this.designModeFocusElement, 'keypress', this.onDesignEditorKeypressHandler);
            this.onDesignEditorKeypressHandler = null;
        }

        $removeHandler(this.dialogFrameElement, 'load', this.onDialogFrameLoadHandler);
        this.onDialogFrameLoadHandler = null;

        $removeHandler(this.htmlModeFocusElement, 'focus', this.onEditorFocusHandler);
        $removeHandler(this.designModeFocusElement, 'focus', this.onEditorFocusHandler);
        this.onEditorFocusHandler = null;

        $removeHandler(this.designModeFocusElement, 'blur', this.onDesignEditorBlurHandler);
        this.onDesignEditorBlurHandler = null;

        $removeHandler(this.htmlModeFocusElement, 'blur', this.onHtmlEditorBlurHandler);
        this.onHtmlEditorBlurHandler = null;
    },

    OnTimerTick: function()
    {
        if (!this._modified) this.CheckModified();
    },

    StartTimer: function()
    {
        if (this.timer == null)
        {
            var interval = 2000;
            this.timer = window.setInterval(Function.createDelegate(this, this.OnTimerTick), interval);
        }
    },

    StopTimer: function()
    {
        if (this.timer != null)
        {
            window.clearInterval(this.timer);
            this.timer = null;
        }
    },

    PreventDefault: function(event)
    {
        if (this.isIE)
            event.returnValue = false;
        else
            event.preventDefault();
    },

    StopPropagation: function(event)
    {
        if (this.isIE)
            event.cancelBubble = true;
        else
            event.stopPropagation();
    },

    GetDesignModeContext: function()
    {
        var context = new Object();

        try
        {
            if (this.designModeDocument.selection)
            {
                context.selection = this.designModeDocument.selection;
                context.range = context.selection.createRange();
                context.selectedText = context.range.text;

                switch (context.selection.type)
                {
                    case "None":
                    case "Text":
                        context.parentElement = context.range.parentElement();
                        break;
                    case "Control":
                        context.parentElement = context.range.item(0);
                        break;
                    default:
                        context.parentElement = this.designModeDocument.body;
                        break;
                }
            }
            else if (this.designModeDocument.getSelection)
            {
                context.selection = this.designEditor.getSelection();
                context.selectedText = context.selection.toString();

                try
                {
                    context.range = context.selection.getRangeAt(0);
                }
                catch (e)
                {
                    context.range = this.designModeDocument.createRange();
                }

                function IsSelectedTextNode(container, offset, start)
                {
                    if (container.nodeType != 3) return false;
                    var startIndex = start ? offset : 0;
                    var endIndex = start ? container.nodeValue.length : offset + 1;
                    var text = container.nodeValue.substring(startIndex, endIndex);

                    return (context.selectedText == text);
                }

                var r = context.range;
                var p = null;

                if (r.startContainer == r.endContainer)
                {
                    if (r.collapsed)
                    {
                        p = r.startContainer;
                    }
                    else if (r.startOffset - r.endOffset <= 1 &&
                             r.startContainer.hasChildNodes())
                    {
                        p = r.startContainer.childNodes[r.startOffset];
                    }
                }
                else if (IsSelectedTextNode(r.startContainer, r.startOffset, true))
                {
                    p = r.startContainer;
                }
                else if (IsSelectedTextNode(r.endContainer, r.endOffset, false))
                {
                    p = r.endContainer;
                }

                if (!p) p = r.commonAncestorContainer;

                while (p.nodeType == 3) p = p.parentNode;

                context.parentElement = p;
            }

            if (context.parentElement == null) return null;
            if (context.parentElement.nodeType != 1) return null;
            if (context.parentElement.ownerDocument != this.designModeDocument) return null;
        }
        catch (e)
        {
            return null;
        }


        return context;
    },

    OnDesignEditorKeyEvent: function(event)
    {
        var e = event || window.event;
        var code = e.charCode || e.keyCode;

        if (e.ctrlKey && !e.altKey && !e.shiftKey)
        {
            var commandName = "";
            switch (String.fromCharCode(code).toLowerCase())
            {
                case 'b':
                    commandName = "bold";
                    break;
                case 'i':
                    commandName = "italic";
                    break;
                case 'u':
                    commandName = "underline";
                    break;
                default:
                    return;
            }

            this.PreventDefault(e);
            this.StopPropagation(e);

            this.ExecuteCommand(commandName, null);
        }

        this.TimeoutInvoke(this.RaiseContextChanged);
    },

    OnDesignEditorMouseEvent: function(event)
    {
        if (this.initialized && !this.designModeInitialized)
        {
            this.InitializeDesignMode();
        }

        this.TimeoutInvoke(this.RaiseContextChanged);
    },

    SetDialogFocus: function()
    {
        if (!this.dialogInfo.current) return;

        var f = $get(this.dialogInfo.elementIDs.dialogFrame);
        var d = f.contentDocument || f.contentWindow.document || f.document;

        if (d.forms.length > 0 && d.forms[0].elements.length > 0)
            d.forms[0].elements[0].focus();
        else
            f.contentWindow.focus();
    },

    OnEditorFocus: function(event)
    {
        if (this.designMode && !this.designModeInitialized)
            this.InitializeDesignMode();

        if (this._modified == false && this.timer == null) this.StartTimer();

        if (this.dialogInfo.current)
            this.TimeoutInvoke(this.SetDialogFocus);
        else
            this.TimeoutInvoke(this.RaiseContextChanged);
    },

    StoreDesignEditor: function()
    {
        this.StopTimer();
        this.CopyDesignToHtml();
    },

    StoreHtmlEditor: function()
    {
        this.StopTimer();
        this.CopyHtmlEditorToHtml();
    },

    AutoSave: function(ctl, args)
    {
        this.Save();
    },

    UpdateHtml: function()
    {
        this.CloseDialogBox();

        var html = this.ClientDecode($get(this.get_htmlencodedTextID()).value);

        if (this.designMode)
        {
            this.StoreHtml(html);
            this.CopyHtmlToDesign();
        }
        else
        {
            if (this.outputXHTML)
                html = this.ConvertHTML(html, false);

            this.StoreHtml(html);
            this.CopyHtmlToHtmlEditor();
        }

        this.SetValue(html, this.designMode);

        this.RecordHtml();
        this.EmptySelection();
    },

    IsDocumentLoaded: function()
    {
        if (this.designMode)
            return this.designModeDocument.body != null;
        else
            return this.htmlModeDocument.body != null;
    },

    OnUpdateHtml: function(sender, args)
    {
        var dataItem = args.get_dataItems()[this.get_htmlencodedTextID()];
        if (dataItem && this.initialized)
        {
            if (this.IsDocumentLoaded())
                this.UpdateHtml();
        }
    }
}

ParsElecom.HtmlEditor.HtmlEditor.descriptor = 
{
    properties: [ {name: 'htmlencodedTextID', type: String },
                  {name: 'modifiedID', type: String },
                  {name: 'htmlEditorID', type: String },
                  {name: 'designEditorID', type: String },
                  {name: 'htmlAreaID', type: String },
                  {name: 'designAreaID', type: String },
                  {name: 'htmlTabID', type: String },
                  {name: 'designTabID', type: String },
                  {name: 'dialogBoxIDs', type: String },
                  {name: 'saveButtonID', type: String },
                  {name: 'toolbarIDs', type: String },
                  {name: 'backColor', type: String },
                  {name: 'editorForeColor', type: String },
                  {name: 'editorBackColor', type: String },
                  {name: 'toolbarColor', type: String },
                  {name: 'tabForeColor', type: String },
                  {name: 'tabBackColor', type: String },
                  {name: 'selectedTabBackColor', type: String },
                  {name: 'selectedTabForeColor', type: String },
                  {name: 'tabMouseOverColor', type: String },
                  {name: 'tabbarBackColor', type: String },
                  {name: 'buttonMouseOverColor', type: String },
                  {name: 'buttonMouseOverBorderColor', type: String },
                  {name: 'dialogColors', type: String },
                  {name: 'outputXHTML', type: Boolean },
                  {name: 'convertDeprecatedSyntax', type: Boolean },
                  {name: 'convertParagraphs', type: Boolean },
                  {name: 'replaceNoBreakSpace', type: Boolean },
                  {name: 'showModifiedAsterick', type: Boolean },
                  {name: 'toolbarData', type: String },
                  {name: 'dialogData', type: String },
                  {name: 'dialogDimensions', type: String },
                  {name: 'pixelImageUrl', type: String },
                  {name: 'allowedTags', type: String },
                  {name: 'allowedAttributes', type: String },
                  {name: 'noScriptAttributes', type: String },
                  {name: 'formatHtmlMode', type: Boolean },
                  {name: 'newLine', type: String },
                  {name: 'initialMode', type: String },
                  {name: 'designModeEditable', type: Boolean },
                  {name: 'htmlModeEditable', type: Boolean },
                  {name: 'designModeCss', type: String },
                  {name: 'designModeEmulateIE7', type: Boolean },
                  {name: 'textDirection', type: String },
                  {name: 'validatorIDs', type: Object },
                  {name: 'version', type: String } ],
                  
    events: [     {name: 'modifiedChanged' },
                  {name: 'contextChanged' } ]
}

ParsElecom.HtmlEditor.HtmlEditor.registerClass("ParsElecom.HtmlEditor.HtmlEditor", Sys.UI.Control);
