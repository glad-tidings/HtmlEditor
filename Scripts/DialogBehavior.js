Type.registerNamespace("ParsElecom.HtmlEditor");

ParsElecom.HtmlEditor.DialogBehavior = function(element)
{
    ParsElecom.HtmlEditor.DialogBehavior.initializeBase(this, [element]);
    
    this._editorControl = null;
    this._dialogDocument = null;
    this._contentWindow = null;
    
    this.options = null;
    this.colors = null;
    this.name = "";
    this.context = null;
    
    this.editor = null;
    this.document = null;
    this.views = null;
    this.tabs = null;
    this.isIE = false;
    this.isFirefox = false;
    this.isOpera = false;
    this.browserVersion = 0;
    
    this.modalState = null;
}

ParsElecom.HtmlEditor.DialogBehavior.prototype = 
{
    get_editorControl: function()
    {
        return this._editorControl;
    },
    
    set_editorControl: function(value)
    {
        this._editorControl = value;
    },
    
    get_dialogDocument: function()
    {
        return this._dialogDocument;
    },
    
    set_dialogDocument: function(value)
    {
        this._dialogDocument = value;
    },
    
    get_contentWindow: function()
    {
        return this._contentWindow;
    },
    
    set_contentWindow: function(value)
    {
        this._contentWindow = value;
    },
    
    ExecuteCommand: function(commandName, commandValue)
    {
        this.editor.ExecuteCommand(commandName, commandValue);
    },
    
    GetDesignModeDocument: function()
    {
        return this.editor.designModeDocument;
    },
    
    IsTagName: function(element, tagName)
    {
        return element.tagName.toLowerCase() == tagName;
    },

    IsParent: function(tagName)
    {
        if (this.context && this.IsTagName(this.context.parentElement, tagName))
            return true;
            
        return false;
    },
    
    IsParentOnList: function(tagNames)
    {
        tagNames = tagNames.split(',');
        
        for (var i = 0; i < tagNames.length; i++)
        {
            if (this.IsParent(tagNames[i])) return true;
        }
        
        return false;
    },

    IsTextBox: function(element)
    {
        return (element && this.IsTagName(element, "input") && element.type == "text");
    },
    
    GetValue: function(propertyElement)
    {
        var value = propertyElement.value.trim();
        return value.length == 0 ? null : value;
    },

    GetBorderWidth: function(borderWidthStyle)
    {
        var widthStyle = borderWidthStyle.trim();
        if (widthStyle.length == 0) return "";
        
        var widths = widthStyle.split(' ');
        var lastWidth = "";
        
        for (var i = 0; i < widths.length; i++)
        {
            var width = this.RemoveUnit(widths[0])
            if (width.length == 0) return "";
            if (lastWidth.length > 0 && width != lastWidth) return "";
            
            lastWidth = width;
        }
        
        return lastWidth;
    },
    
    SetElementColors: function(elements, borderColor, backgroundColor, color)
    {
        if (!elements) return;
        
        for (var i = 0; i < elements.length; i++)
        {
            var element = elements[i];
            if (borderColor) element.style.borderColor = borderColor;
            if (backgroundColor) element.style.backgroundColor = backgroundColor;
            if (color) element.style.color = color;
        }
    },
    
    FindOption: function(select, value)
    {
        var findValue = value.trim().toLowerCase();
        var options = select.getElementsByTagName("option");
        
        for (var i = 0; i < options.length; i++)
        {
            if (options[i].value == findValue) return findValue;
        }
        
        return "";
    },
    
    FindTabIndex: function(tab)
    {
        for (var i = 0; i < this.tabs.length; i++)
        {
            if (this.tabs[i] == tab) return i;
        }
        
        return -1;
    },
    
    SelectLabel: function(element)
    {
        var selectedColor = this.colors.selectedTab;
        var unselectedColor = this.colors.table;
        var selectedTextColor = this.colors.selectedTabText;
        var unselectedTextColor = this.colors.text;
        
        var p = element.parentNode;
        while (p.tagName.toLowerCase() != "tr") p = p.parentNode;
        
        var t = p.parentNode;
        for (var i = 0; i < t.childNodes.length; i++)
        {
            var childNode = t.childNodes[i];
            if (childNode.nodeType != 1) continue;
            if (childNode.tagName.toLowerCase() != "tr") continue;
            
            var cells = childNode.getElementsByTagName("td");
            if (cells.length != 2) continue;
            
            var selected = p == childNode;
            cells[0].style.backgroundColor = selected ? selectedColor : unselectedColor;
            cells[0].style.fontWeight = selected ? "bold" : "normal";
            cells[0].style.color = selected ? selectedTextColor : unselectedTextColor;
            
            function FindFocusChild(parentNode)
            {
                var childNodes = parentNode.childNodes;
                
                for (var ii = 0; ii < childNodes.length; ii++)
                {
                    var node = childNodes[ii];
                    if (node.nodeType != 1) continue;
                    
                    var tagName = node.tagName.toLowerCase();
                    if (tagName != "input" && tagName != "select") continue;
                    
                    if (node.focus) return node;
                }
                
                return null;
            }

            if (selected && this.IsTagName(element, "td"))
            {
                var focusChild = FindFocusChild(cells[1]);
                
                if (focusChild)
                    focusChild.focus();
            }
        }
    },
    
    SetTabs: function(selectedTabID)
    {
        var selectedTabColor = this.colors.selectedTab;
        var unselectedTabColor = this.colors.unselectedTab;

        var selectedTabTextColor = this.colors.selectedTabText;
        var unselectedTabTextColor = this.colors.tabText;

        for (var i = 0; i < this.tabs.length; i++)
        {
            var tab = this.tabs[i];
            var selected = tab.id == selectedTabID;

            tab.style.backgroundColor = selected ? selectedTabColor : unselectedTabColor;
            tab.style.color = selected ? selectedTabTextColor : unselectedTabTextColor;
            tab.style.fontWeight = selected ? "bold" : "normal";
        }
    },
    
    RoundTabCorners: function()
    {
        for (var i = 0; i < this.tabs.length; i++)
        {
            var tab = this.tabs[i];
            var leftBottom = tab.ownerDocument.createElement("img");
            var rightBottom = tab.ownerDocument.createElement("img");
            
            var corners = [ leftBottom, rightBottom ];
            
            for (var ii = 0; ii < corners.length; ii++)
            {
                var corner = corners[ii];
                corner.src = this.options.pixelImageUrl;
                corner.height = 1;
                corner.width = 1;
                corner.alt = "";
                corner.style.position = "absolute";
                corner.style.bottom = this.isIE && this.browserVersion < 7 ? "-1px" : "0px";
                corner.style.margin = "0px";
                corner.style.padding = "0px";
                corner.style.backgroundColor = this.colors.body;
                corner.style.fontSize = "0px";
            }
            
            leftBottom.style.left = "0px";
            rightBottom.style.right = "0px";
            
            tab.appendChild(leftBottom);
            tab.appendChild(rightBottom);
        }
    },
    
    FindParent: function(tagName)
    {
        if (this.context == null) return null;
        if (this.IsParent(tagName)) return this.context.parentElement;
        
        if (!this.editor.isIE)
        {
            var sc = this.context.range.startContainer;
            
            if (sc.nodeType == 3 && sc.parentNode.nodeType == 1 && this.IsTagName(sc.parentNode, tagName))
            {
                var r = this.editor.designModeDocument.createRange();
                r.selectNode(sc.parentNode);
                this.context.selection.removeAllRanges();
                this.context.selection.addRange(r);
                this.context.range = r;
                this.context.parentElement = sc.parentNode;
                
                return sc.parentNode; 
            }    
        }
        
        return null;
    },
    
    RemoveUnit: function(valueUnit)
    {
        valueUnit = valueUnit.trim().toLowerCase();
        if (valueUnit.length == 0) return "";
        
        var number = parseInt(valueUnit);
        if (typeof number != "number") return "";

        valueUnit = number.toString();
        
        return valueUnit == "NaN" ? "" : valueUnit;
    },

    OnTextBoxFocus: function(tb)
    {
        this.SelectLabel(tb);
        this.SetElementColors([ tb ], this.colors.selectedTab, this.colors.heading, this.colors.headingText);
    },

    OnTextBoxBlur: function(tb)
    {
        switch (tb.className)
        {
            case "size" :
            case "unit" :
                tb.value = this.RemoveUnit(tb.value);
                break;
            default :
                break;
        }
        
        this.SetElementColors([ tb ], this.colors.body, this.colors.body, this.colors.text);
    },

    OnSelectBoxChange: function(sb, otherValue, otherTextBoxID)
    {
        var showTextBox = sb.value == otherValue ? true : false;
        var otherTextBox = this.document.getElementById(otherTextBoxID);
        otherTextBox.style.display = showTextBox ? "inline" : "none";
        
        if (showTextBox) otherTextBox.focus();
    },
    
    OnTab: function(tab)
    {
        var tabIndex = this.FindTabIndex(tab);
        
        if (this.window.OnBeforeViewChange)
            this.window.OnBeforeViewChange(this.views[tabIndex].id);
        
        for (var i = 0; i < this.tabs.length; i++)
        {
            var selected = i == tabIndex;
            this.views[i].style.display = selected ? "block" : "none";
        }

        if (this.window.OnAfterViewChange)
            this.window.OnAfterViewChange(this.views[tabIndex].id);
        
        this.SetTabs(tab.id);
    },

    UpdateContext: function()
    {
        this.context = this.editor.GetContext();
    },

    Focus: function(elementID)
    {
        this.document.getElementById(elementID).focus();
    },
    
    GetViews: function(viewIDs)
    {
        var IDs = viewIDs.split(',');
        var viewList = new Array(IDs.length);
        
        for (var i = 0; i < IDs.length; i++)
        {
            viewList[i] = this.document.getElementById(IDs[i]);
        }
        
        return viewList;
    },
    
    GetTabs: function(tabIDs)
    {
        var IDs = tabIDs.split(',');
        var tabList = new Array(IDs.length);
        
        for (var i = 0; i < IDs.length; i++)
        {
            tabList[i] = this.document.getElementById(IDs[i]);
        }
        
        return tabList;
    },
    
    InitializeDocument: function()
    {
        this.document.body.style.backgroundColor = this.colors.body;
        this.document.body.style.color = this.colors.text;
    },
    
    InitializeViews: function(viewsID, viewIDs)
    {
        this.document.getElementById(viewsID).style.borderColor = this.colors.selectedTab;
        this.views = this.GetViews(viewIDs);
    },
    
    InitializeTabs: function(tabbarID, tabIDs)
    {
        this.document.getElementById(tabbarID).style.color = this.colors.tabText;

        this.tabs = this.GetTabs(tabIDs);
        this.RoundTabCorners();
        this.SetTabs(this.tabs[0].id);
    },
    
    GetTableInputElements: function(table)
    {
        var inputs = new Array();
        var elements = table.getElementsByTagName("input");
        
        for (var i = 0; i < elements.length; i++)
        {
            if (elements[i].type.toLowerCase() == "button") continue;
            Array.add(inputs, elements[i]);
        }
        
        return inputs;
    },
    
    InitializeTables: function(tableIDs)
    {
        var IDs = tableIDs.split(',');
        
        for (var i = 0; i < IDs.length; i++)
        {
            var table = this.document.getElementById(IDs[i]);
            table.style.backgroundColor = this.colors.table;
            table.style.color = this.colors.text;
            
            this.SetElementColors(table.getElementsByTagName("td"), this.colors.heading);
            this.SetElementColors(this.GetTableInputElements(table), this.colors.body, this.colors.body, this.colors.text);
            this.SetElementColors(table.getElementsByTagName("select"), this.colors.selectedTab, this.colors.heading, this.colors.headingText);
        }
    },
    
    initialize: function()
    {
        ParsElecom.HtmlEditor.DialogBehavior.callBaseMethod(this, 'initialize');

        this.editor = this.get_editorControl();
        this.document = this.get_dialogDocument();
        this.window = this.get_contentWindow();

        this.options = this.editor.dialogInfo.options;
        this.colors = this.options.colors;
        this.name = this.editor.dialogInfo.current.name;
        this.context = this.editor.dialogInfo.current.context;
        
        this.isIE = this.editor.isIE;
        this.isFirefox = this.editor.isFirefox;
        this.isOpera = this.editor.isOpera;
        this.browserVersion = this.editor.browserVersion;
    },
    
    dispose: function()
    {
        ParsElecom.HtmlEditor.DialogBehavior.callBaseMethod(this, 'dispose');
    },
    
    EnableElements: function(elements, enabled)
    {
        for (var i = 0; i < elements.length; i++)
        {
            elements[i].disabled = enabled ? false : true;
        }
    },
    
    SetShield: function(shield)
    {
        if (shield)
        {
            this.modalState = new Object();
            this.modalState.tabElements = this.editor.DisableTabElements(this.document.body);

            var dialogBox = $get(this.editor.dialogInfo.elementIDs.dialogBox); 
            this.EnableElements(dialogBox.getElementsByTagName("input"), false);

            this.modalState.modalShield = this.editor.CreateShield(dialogBox, dialogBox.offsetHeight, dialogBox.offsetWidth);
        }
        else if (this.modalState)
        {
            this.editor.RemoveShield(this.modalState.modalShield);
            this.editor.RestoreTabElements(this.modalState.tabElements);

            var dialogBox = $get(this.editor.dialogInfo.elementIDs.dialogBox); 
            this.EnableElements(dialogBox.getElementsByTagName("input"), true);
            
            this.modalState = null;
        }
    }
}

ParsElecom.HtmlEditor.DialogBehavior.descriptor = 
{
    properties: [ {name: 'editorControl', type: Object },
                  {name: 'dialogDocument', type: Object },
                  {name: 'contentWindow', type: Object } ]
}

ParsElecom.HtmlEditor.DialogBehavior.registerClass("ParsElecom.HtmlEditor.DialogBehavior", Sys.UI.Behavior);
