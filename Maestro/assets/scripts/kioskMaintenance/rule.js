var variable_operator_div_opened = false; //Flag to open variable operator div
var global_default_modal_opened = false; //Flag to open global default setting

/*
Load a list of rules from the database 
-Load from 3 tables - BR_RULE_LIST, BR_RULE_EXPRESSION, M_MACHINE_RULES
*/
function fn_getRulesList() {
    var str = "";
    var first = true;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getRulesList",
        data: "{}",
        dataType: "json",
        success: function (data) {

            for (var i = 0; i < data.d.length; i++) {
                str = "";

                /*
                if is first record, do not need to compare with previous row, just populate the data
                */
                if (first) {
                    first = false;

                    if ($.trim(data.d[i].UserViewExpr) == "") {
                        str = str + '<tr><td><div id="' + data.d[i].RuleID + '" class="global-default-rule incomplete-rule">';
                    }
                    else {
                        str = str + '<tr><td><div id="' + data.d[i].RuleID + '" class="global-default-rule complete-rule">';
                    }

                    str = str + '<div class="gd-rule-name">' + data.d[i].RuleName + '</div>';

                    if ($.trim(data.d[i].UserViewExpr) == "") {
                        str = str + '<div class="gd-rule-value"> - </div>';
                    }
                    else {
                        if ($.trim(data.d[i].RuleID) == '19' || $.trim(data.d[i].RuleID) == '20') {
                            str = str + '<div class="gd-rule-value"> - </div>';
                        }
                        else {
                            str = str + '<div class="gd-rule-value">' + data.d[i].UserViewExpr + '</div>';
                        }
                    }

                    str = str + '</div></td>';
                    str = str + '<td><div class="specific-rule-column">';

                    if ($.trim(data.d[i].SpecificMach) !== "" && $.trim(data.d[i].SpecificUVExpr) !== "") {
                        str = str + '<div class="gd-specific"><i class="fa fa-trash-o delete-gd-specific-icon"></i><div class="gd-specific-machine">' + data.d[i].SpecificMach + '</div>'
                        str = str + '<div class="gd-specific-value">' + data.d[i].SpecificUVExpr + '</div></div>'
                    }

                    if ($.trim(data.d[i].RuleID) == '19' || $.trim(data.d[i].RuleID) == '20') {
                        str = str + "No Exception";
                    }
                    else {
                        if ($.trim(data.d[i].UserViewExpr) == "") {
                            str = str + 'N/A';
                        }
                        else {
                            str = str + '<a>Click if Kiosk rule differs</a>';
                        }
                    }

                    str = str + '</div></td></tr>';
                    $('#rule-assignment-table > tbody').empty();
                    $('#rule-assignment-table > tbody').append(str);
                }
                /*
                If is not first record, compare current record with previous record
                - If RuleID not same as previous RuleID, continue populate the data
                */
                else if (data.d[i].RuleSeq !== data.d[i - 1].RuleSeq) {

                    if ($.trim(data.d[i].UserViewExpr) == "") {
                        str = str + '<tr><td><div id="' + data.d[i].RuleID + '" class="global-default-rule incomplete-rule">';
                    }
                    else {
                        str = str + '<tr><td><div id="' + data.d[i].RuleID + '" class="global-default-rule complete-rule">';
                    }

                    str = str + '<div class="gd-rule-name">' + data.d[i].RuleName + '</div>';

                    if ($.trim(data.d[i].UserViewExpr) == "") {
                        str = str + '<div class="gd-rule-value"> - </div>';
                    }
                    else {
                        if ($.trim(data.d[i].RuleID) == '19' || $.trim(data.d[i].RuleID) == '20') {
                            str = str + '<div class="gd-rule-value"> - </div>';
                        }
                        else {
                            str = str + '<div class="gd-rule-value">' + data.d[i].UserViewExpr + '</div>';
                        }
                    }

                    str = str + '</div></td>';
                    str = str + '<td><div class="specific-rule-column">';

                    if ($.trim(data.d[i].SpecificMach) !== "" && $.trim(data.d[i].SpecificUVExpr) !== "") {
                        str = str + '<div class="gd-specific"><i class="fa fa-trash-o delete-gd-specific-icon"></i><div class="gd-specific-machine">' + data.d[i].SpecificMach + '</div>'
                        str = str + '<div class="gd-specific-value">' + data.d[i].SpecificUVExpr + '</div></div>'
                    }

                    if ($.trim(data.d[i].RuleID) == '19' || $.trim(data.d[i].RuleID) == '20') {

                        //Split USER_VIEW_EXPR and populate correctly
                        var userViewExpr = data.d[i].UserViewExpr.split(";");

                        for (var j = 0; j < userViewExpr.length; j++) {
                            if (userViewExpr[j] !== "") {
                                var groupName = userViewExpr[j].substring(0, userViewExpr[j].indexOf(':'));
                                var expr = userViewExpr[j].substring(userViewExpr[j].indexOf(':') + 1);

                                str = str + '<div class="setup-group-name">' + groupName + '<div class="setup-group-expr" style="display:none;">' + expr + '</div></div>';
                            }
                        }

                        /* Split RULE_EXPR and populate correctly
                        var expr = data.d[i].RuleExpression.split(";");

                        for (var j = 0; j < expr.length; j++) {
                        if (expr[j] !== "") {
                        expr[j] = expr[j] + ';';
                        var groupName = expr[j].substring(expr[j].indexOf('"') + 1);

                        groupName = groupName.substring(0, groupName.indexOf('"'));

                        str = str + '<div class="setup-group-name">' + groupName + '<div class="setup-group-expr" style="display:none;">' + expr[j] + '</div></div>';
                        }
                        }
                        */
                    }
                    else {
                        if ($.trim(data.d[i].UserViewExpr) == "") {
                            str = str + 'N/A';
                        }
                        else {
                            str = str + '<a>Click if Kiosk rule differs</a>';
                        }
                    }

                    str = str + '</div></td></tr>';

                    $('#rule-assignment-table > tbody').append(str);
                }
                /*
                If current RuleID same as previous RuleID
                -Compare current specific expr, if same then group them, if not then create another div
                */
                else if (data.d[i].RuleSeq == data.d[i - 1].RuleSeq) {

                    if ($.trim(data.d[i].SpecificMach) !== "" && $.trim(data.d[i].SpecificUVExpr) !== "") {

                        if ($.trim(data.d[i].SpecificUVExpr) !== $.trim(data.d[i - 1].SpecificUVExpr)) {
                            $('.global-default-rule').last().closest('tr').find('.specific-rule-column').find('a').before('<div class="gd-specific"><i class="fa fa-trash-o delete-gd-specific-icon"></i><div class="gd-specific-machine">' + data.d[i].SpecificMach + '</div>'
                                                                                                                        + '<div class="gd-specific-value">' + data.d[i].SpecificUVExpr + '</div></div>');
                        }
                        else {
                            $('.global-default-rule').last().closest('tr').find('.specific-rule-column .gd-specific-value').each(function () {

                                if (data.d[i].SpecificUVExpr == $(this).text()) {

                                    $(this).closest('.gd-specific').find('.gd-specific-machine').text($(this).closest('.gd-specific').find('.gd-specific-machine').text() + ', ' + data.d[i].SpecificMach);
                                }
                            });
                        }
                    }
                }
                else {
                    continue;
                }
            }
        },
        error: function (error) {
            alert("Error occurs when trying to loading rules list.");
        }
    });
}

//Click on a particular global default rule, append popup modal into #content-mainpage-container (index.aspx) and populate the values.
function gd_getRuleDetail() {
    $('#rule-assignment').on('click', '.global-default-rule', function () {
        var str = "";
        var ruleID = $(this).attr('id');

        if (!global_default_modal_opened) {

            global_default_modal_opened = true;

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getRuleDetail",
                data: "{ruleID : '" + ruleID + "'}",
                dataType: "json",
                success: function (data) {
                    str = str + '<div id="modal-bg-opacity"></div>';
                    str = str + '<div id="global-default-rule-modal">';
                    str = str + '<div id="global-default-rule-bg"></div>';
                    str = str + '<div id="global-default-rule-config-div">';
                    str = str + '<div id="gd-config-header">';
                    str = str + '<span class="close-div-icon" id="close-gd-modal"><i class="fa fa-times"></i></span>';
                    str = str + '<p class="config-rule-label">' + data.d[0].RuleLabel + '</p> <hr /></div>';
                    str = str + '<div id="gd-config-body">';
                    str = str + '<table><tbody><tr style="display:none;"><td id="gd-rule-id">' + data.d[0].RuleID + '</td></tr>';

                    var isCondtion = (data.d[0].IsCondition).toUpperCase();

                    if (isCondtion == 'TRUE') {
                        str = str + '<tr id="gd-condition"><td><label>Condition</label></td>';
                        str = str + '<td><div id="inputCondition" data-value="" data-variable="" data-temp=""></div></td></tr>';

                    }

                    var resultType = (data.d[0].ResultType).toUpperCase();

                    switch (resultType) {
                        case "I":
                            /* Future 
                            **********
                            ** Add a validation check if min & max is empty, 
                            ** if is empty, then do not need to have min and max attributes. 
                            ** (Specific for Rule 17, 18 which do not have a valid number range)
                            ** For generic validation purpose 
                            **********
                            */
                            str = str + '<tr><td><label id="gd-return-label"><label></td>';
                            str = str + '<td id="gd-return-section">';
                            str = str + '<div result-type="I"><input type="text" id="inputGDReturn" min="' + data.d[0].IntegerMin + '" max="' + data.d[0].IntegerMax + '" result-type="I" value="' + data.d[0].RuleExpression + '" /><span class="gd-return-error-msg" style="display:none;"></span></div>';
                            str = str + '</td></tr>';
                            break;

                        case "B":

                            if ($.trim(data.d[0].RuleExpression) !== "") {

                                if ((data.d[0].RuleExpression).toLowerCase() == "true") {
                                    str = str + '<tr><td><label id="gd-return-label"><label></td>';
                                    str = str + '<td id="gd-return-section">';
                                    str = str + '<div result-type="B"><input type="checkbox" class="B-checkbox" value="true" checked="checked" /><label>Yes</label><input type="checkbox" class="B-checkbox" value="false" /><label>No</label><span class="gd-return-error-msg" style="display:none;"></span></div>';
                                    str = str + '</td></tr>';
                                }
                                else {
                                    str = str + '<tr><td><label id="gd-return-label"><label></td>';
                                    str = str + '<td id="gd-return-section">';
                                    str = str + '<div result-type="B"><input type="checkbox" class="B-checkbox" value="true" /><label>Yes</label><input type="checkbox" class="B-checkbox" value="false" checked="checked" /><label>No</label><span class="gd-return-error-msg" style="display:none;"></span></div>';
                                    str = str + '</td></tr>';
                                }
                            }
                            else {
                                str = str + '<tr><td><label id="gd-return-label"><label></td>';
                                str = str + '<td id="gd-return-section">';
                                str = str + '<div result-type="B"><input type="checkbox" class="B-checkbox" value="true" /><label>Yes</label><input type="checkbox" class="B-checkbox" value="false" /><label>No</label><span class="gd-return-error-msg" style="display:none;"></span></div>';
                                str = str + '</td></tr>';
                            }

                            break;

                        case "E":
                            str = str + '<tr><td><label id="gd-return-label">Go To<label></td>';
                            str = str + '<td id="gd-return-section">';
                            str = str + '<div result-type="E"><select><option value="1">Bin 1</option><option value="2">Bin 2</option><option value="3">Bin 3</option><option value="4">Bin 4</option></select></div>';
                            str = str + '</td></tr>';

                            str = str + '<tr><td></td><td><button class="btn btn-primary" id="gd-add-expression-btn">Add</button></td></tr>';

                            if ($.trim(data.d[0].RuleExpression) !== "") {
                                var userExprArray = data.d[0].UserViewExpr.split(";");
                                var exprArray = data.d[0].RuleExpression.split(";");

                                str = str + '<tr id="expression-list-row"><td><label>Expression</label></td><td><div id="expression-list-div">'

                                for (var i = 0; i < userExprArray.length; i++) {
                                    if (userExprArray[i] !== "" && exprArray[i] !== "") {

                                        str = str + '<div class="expression"><span class="remove-expression-icon"><i class="fa fa-trash-o"></i></span>'
                                        str = str + '<div class="expression-userView">' + userExprArray[i] + '</div><div class="expression-value" style="display:none;">' + exprArray[i] + ';</div>'
                                        str = str + '</div>';
                                    }
                                }

                                str = str + '</div></td></tr>';
                            }
                            else {
                                str = str + '<tr id="expression-list-row"><td><label>Expression</label></td><td><div id="expression-list-div"></div></td></tr>';
                            }
                            break;

                        case "T":
                            str = str + '<tr><td><label id="gd-return-label"><label></td>';
                            str = str + '<td id="gd-return-section">';
                            str = str + '<div result-type="T"><input id="inputTime" type="time" value="' + data.d[0].RuleExpression + '" /><span class="gd-return-error-msg" style="display:none;"></span></div>';
                            str = str + '</td></tr>';
                            break;

                        case "D":

                            if (data.d[0].RuleID == '19') {
                                str = str + '<tr style="display:none;"><td><div id="pre-defined-variable-name">AccountNo</div></td></tr>';
                            }
                            if (data.d[0].RuleID == '20') {
                                str = str + '<tr style="display:none;"><td><div id="pre-defined-variable-name">BankCode</div></td></tr>';
                            }

                            str = str + '<tr><td><label>Name</label></td><td><input id="inputGroupName" type="text" /></td></tr>';
                            str = str + '<td id="gd-return-section" style="display:none;"><div result-type="D"></div></div>';

                            str = str + '<tr><td><select class="D-selection">'
                            str = str + '<option value="equal">Equals to</option><option value="within">Within range</option></select>';
                            str = str + '</td><td class="gd-return-section"><input type="text" /><span class="gd-return-error-msg" style="display:none;"></span></td>';
                            str = str + '</tr>';
                            str = str + '<tr><td><a id="add-more-condition">add condition</a></td></tr>';

                            str = str + '<tr><td></td><td><button class="btn btn-primary" id="gd-add-expression-btn">Add</button>';
                            str = str + '<div id="gd-update-section" style="display:none;"><button class="btn btn-primary" id="gd-update-expression-btn">Update</button>&nbsp;<button class="btn btn-default" id="gd-cancel-update-btn" >Cancel</button></div></td></tr>';

                            if ($.trim(data.d[0].RuleExpression) !== "") {
                                var userExprArray = data.d[0].UserViewExpr.split(";");
                                var exprArray = data.d[0].RuleExpression.split(";");

                                str = str + '<tr id="expression-list-row"><td><label>Expression</label></td><td><div id="expression-list-div">'

                                for (var i = 0; i < userExprArray.length; i++) {
                                    if (userExprArray[i] !== "" && exprArray[i] !== "") {

                                        str = str + '<div class="expression exp-editable"><span class="remove-expression-icon"><i class="fa fa-trash-o"></i></span>'
                                        str = str + '<div class="expression-userView">' + userExprArray[i] + '</div><div class="expression-value" style="display:none;">' + exprArray[i] + ';</div>'
                                        str = str + '</div>';
                                    }
                                }
                                /*
                                for (var i = 0; i < exprArray.length; i++) {
                                if (exprArray[i] !== "") {
                                str = str + '<div class="expression"><span class="remove-expression-icon"><i class="fa fa-trash-o"></i></span>'
                                str = str + exprArray[i] + ';' + '</div>';
                                }
                                }
                                */

                                str = str + '</div></td></tr>';
                            }
                            else {
                                str = str + '<tr id="expression-list-row"><td><label>Expression</label></td><td><div id="expression-list-div"></div></td></tr>';
                            }

                            break;
                    }

                    str = str + '<tr><td></td><td><div class="remark-section"><span class="rule-remark">' + data.d[0].RuleRemark + '</span></div></td></tr>';
                    str = str + '<tr id="gd-button-section"><td></td><td><button class="btn btn-primary" access-gate task="Rule Assignment" permission="Edit" id="save-gd-btn">Save</button>&nbsp;';
                    str = str + '<button class="btn btn-default" id="close-gd-btn">Close</button></td></tr>';
                    str = str + '</tbody></table></div></div></div>';

                    $('body').css('overflow', 'hidden');
                    $('#rule-assignment').append(str);
                    $('#inputGDReturn').focus();

                    //Get variable list from database and check in the string, if exist, put it in the inputCondition attribute (data-variable) for later save into database purpose
                    if (resultType == "E") {

                        var str1 = "";
                        var variables = $('#inputCondition').attr('data-variable');

                        $('#expression-list-div .expression').each(function () {
                            str1 = str1 + $(this).text();
                        });

                        $.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getVariableList",
                            data: "{}",
                            dataType: "json",
                            success: function (data) {

                                for (var i = 0; i < data.d.length; i++) {
                                    if (str1.indexOf(data.d[i].VariableName) > -1) {
                                        variables = variables + data.d[i].VariableName + ',';
                                    }
                                }

                                $('#inputCondition').attr('data-variable', variables);
                            },
                            error: function (error) {
                                alert("Error occurs when trying to load the variable list.");
                            }
                        });
                    }
                },
                error: function (error) {
                    alert("Error occurs when trying to load the particular rule.");
                    $('#rule-loader-gif').remove();
                    global_default_modal_opened = false;
                }
            });
        }
    });
}

function fn_VariableOperator() {

    var str1 = "";

    $('#rule-assignment').on('click', '#inputCondition', function (event) {
        event.stopPropagation();

        if (!variable_operator_div_opened) {
            variable_operator_div_opened = true;
            $('#inputCondition').empty();
            $('#inputCondition').attr('data-value', '(');

            //Hardcode test
            str1 = str1 + '<div class="a-condition">';

            $(this).after('<div id="variable-operator-popup" style="display:none;">'
                        + '<div id="variable-div">'
                        + '<table><tbody><tr><td>'
                        + '<div id="variable-name-div">'
                        + '<h4>Select Variable</h4><hr/>'
                        + '<div id="variable-name-list"></div></td>'
                        + '<td id="variable-value-column"><div id="variable-value-div"><h4>Enter Value<div></h4><hr/><div><input type="text" id="inputVariableValue" /><button>Enter</button></div></div></td></tr></tbody></table>'
                        + '</div>'
                        + '<div id="operator-div" style="display:none;">'
                        + '<div id="operator-list-div"><h4>Select Operator</h4><hr/><div id="operator-list"></div></div>'
                        + '</div>');

            $('#variable-operator-popup').show();

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getVariableList",
                data: "{}",
                dataType: "json",
                beforeSend: function () {
                    $('#variable-name-list').html('Loading...');
                },
                success: function (data) {
                    $('#variable-name-list').empty();

                    for (var i = 0; i < data.d.length; i++) {
                        $('#variable-name-list').append('<div class="variable-name"><span>' + data.d[i].VariableName + '</span></div>');
                    }
                },
                error: function (error) {
                    alert("Error occurs when trying to load the variable list.");
                    variable_operator_div_opened = false;
                }
            });

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getOperatorList",
                data: "{}",
                dataType: "json",
                success: function (data) {
                    for (var i = 0; i < data.d.length; i++) {
                        $('#operator-list').append('<div class="operator-desc"><span operator-type="' + data.d[i].OperatorType + '">' + data.d[i].OperatorDesc + '</span></div>');
                    }
                },
                error: function (error) {
                    alert("Error occurs when trying to load the operator list.");
                    variable_operator_div_opened = false;
                }
            });
        }
    });

    $('#rule-assignment').on('click', '#variable-operator-popup', function (event) {
        event.stopPropagation();
    });

    $('#rule-assignment').on('click', '#global-default-rule-config-div, #machine-list-main-div', function () {
        $('#variable-operator-popup').remove();
        variable_operator_div_opened = false;
    });

    $('#rule-assignment').on('click', '.variable-name > span', function () {
        var inputGD_value = $('#inputCondition').attr('data-value');
        var inputGD_variable = $('#inputCondition').attr('data-temp');

        inputGD_value = inputGD_value + $(this).text() + " ";

        $('#inputCondition').attr('data-value', inputGD_value);
        $('#inputCondition').append($(this).text() + " ");

        //Add variable into attributes
        if (inputGD_variable.indexOf($(this).text()) == -1) {
            inputGD_variable = inputGD_variable + $(this).text() + ",";
            $('#inputCondition').attr('data-temp', inputGD_variable);
        }

        $('#variable-div').hide();
        $('#operator-div').show();
    });

    $('#rule-assignment').on('click', '#variable-value-div button', function () {
        var inputVV = $('#inputVariableValue').val();
        var inputGD_value = $('#inputCondition').attr('data-value');

        if (inputVV !== "") {
            inputGD_value = inputGD_value + '"' + inputVV + '" ';

            $('#inputCondition').attr('data-value', inputGD_value);
            $('#inputCondition').append('"' + inputVV + '" ');

            $('#inputVariableValue').val("");
            $('#variable-div').hide();
            $('#operator-div').show();
        }
        else {
            alert("Please enter the value or choose from the variable list.");
        }
    });

    $('#rule-assignment').on('click', '.operator-desc > span', function () {
        var inputGD = $('#inputCondition').text();
        var inputGD_value = $('#inputCondition').attr('data-value');
        var operatorType = $(this).attr('operator-type');

        if ($(this).text().toUpperCase() == "AND" || $(this).text().toUpperCase() == "OR") {

            inputGD_value = inputGD_value + ") " + operatorType + " ( ";

            $('#inputCondition').append('<span style="font-weight:bold;font-style:italic;">' + $(this).text().toUpperCase() + '</span> ');
            $('#inputCondition').append('<br/>');
        }
        else {
            inputGD_value = inputGD_value + operatorType + " ";
            $('#inputCondition').append($(this).text() + " ");
        }

        $('#inputCondition').attr('data-value', inputGD_value);

        $('#operator-div').hide();
        $('#variable-div').show();
    });
}

function gd_addExpression() {

    $('#rule-assignment').on('click', '#gd-add-expression-btn', function () {

        var result_type = $('#gd-return-section > div').attr('result-type');
        var str = "";
        var userView = "";
        var ready = true;
        var action = "add";

        switch (result_type.toUpperCase()) {
            case "E":

                var inputConditionValue = $('#inputCondition').attr('data-value');
                var setTo = $('#gd-return-section select').val();

                var conditionText = $('#inputCondition').text();
                var goTo = $('#gd-return-section option:selected').text();

                if ($('#inputCondition').text() == "") {
                    alert("Please input condition");

                    return false;
                }

                if (setTo !== null) {
                    str = str + 'if (' + inputConditionValue + ')) ';
                    str = str + 'return "' + setTo + '";';

                    userView = userView + conditionText + ', Go to "' + goTo + '"';
                    gd_ajax_parseStatement(userView, str, gd_resetE);
                }
                else {
                    alert("Please select.");
                    $('#gd-return-section select').focus();
                }

                break;

            case "D":

                var variableName = $('#pre-defined-variable-name').text();
                var groupName = $.trim($('#inputGroupName').val());

                $('.D-selection').each(function () {
                    if ($(this).val() == 'equal') {
                        if ($.trim($(this).closest('tr').find('.gd-return-section input').first().val()) == "") {
                            $(this).closest('tr').find('.gd-return-section input').last().next().html('Please do not leave it blank.').show();

                            ready = false;
                        }
                        else {
                            $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                        }
                    }

                    if ($(this).val() == 'within') {
                        if ($.trim($(this).closest('tr').find('.gd-return-section input').first().val()) == "" || $.trim($(this).closest('tr').find('.gd-return-section input').last().val()) == "") {
                            $(this).closest('tr').find('.gd-return-section input').last().next().html('Please do not leave it blank.').show();

                            ready = false;
                        }
                        else {
                            $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                        }

                    }
                });

                if (groupName == "" || groupName == null) {
                    $('#inputGroupName').after('<span class="gd-return-error-msg">Please do not leave it blank.</span>');

                    ready = false;
                }
                else {
                    $('#inputGroupName').next().remove();
                }

                if ($('#expression-list-div .expression').length !== 0) {

                    $('#expression-list-div .expression').each(function () {
                        var EV_groupName = $(this).find('.expression-value').text();
                        EV_groupName = EV_groupName.substring(EV_groupName.indexOf('"') + 1);
                        EV_groupName = EV_groupName.substring(0, EV_groupName.indexOf('"'));

                        var UV_groupName = $(this).find('.expression-userView').text();
                        UV_groupName = UV_groupName.substring(0, UV_groupName.indexOf(':'));

                        if (groupName.trim() == UV_groupName.trim()) {
                            $('#inputGroupName').after('<span class="gd-return-error-msg">Group name has been used. Please enter another.</span>');

                            ready = false;
                        }
                        else {
                            $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                        }
                    });
                }

                //If all set
                if (ready) {

                    str = str + "if ";
                    str = str + "(";

                    userView = userView + groupName + ' : ';

                    $('.D-selection').each(function () {
                        if ($(this).val() == 'equal') {
                            $(this).closest('tr').find('.gd-return-section input').first().next().hide();
                            str = str + '(' + variableName + ' == ' + $.trim($(this).closest('tr').find('.gd-return-section input').first().val()) + ') || ';

                            userView = userView + $.trim($(this).closest('tr').find('.gd-return-section input').first().val()) + ', ';
                        }

                        if ($(this).val() == 'within') {
                            $(this).closest('tr').find('.gd-return-section input').last().next().hide();

                            str = str + '((' + variableName + ' >= ' + $(this).closest('tr').find('.gd-return-section input').first().val() + ') && ';
                            str = str + '(' + variableName + ' <= ' + $(this).closest('tr').find('.gd-return-section input').last().val() + ')) || ';

                            userView = userView + $.trim($(this).closest('tr').find('.gd-return-section input').first().val());
                            userView = userView + " to " + $.trim($(this).closest('tr').find('.gd-return-section input').last().val()) + ', ';
                        }
                    });

                    str = str.trim();
                    str = str.substring(0, str.length - 2);
                    str = str.trim();

                    str = str + ')';
                    str = str + ' return ' + '"' + groupName + '"' + ';';

                    userView = userView.trim();
                    userView = userView.substring(0, userView.length - 1);
                    userView = userView.trim();

                    group_ajax_parseStatement(action, userView, str, gd_resetD);
                }

                break;
        }
    });
}
//This function only applicable for Setup Group (Account & Cheque)
function group_editable() {
    $('#rule-assignment').on('click', '.exp-editable', function () {
        $('.gd-return-error-msg').hide();
        $('#expression-list-div').find('.exp-editable').removeClass('exp-active');
        $(this).addClass('exp-active');

        var exp = $(this).find('.expression-userView').text(); //Get the entire expression. Example = Account Name: 100 to 200, 500, 505 to 888
        var array = exp.split(':'); //Split the entire expression into two. 1.) Account Name 2.) 100 to 200, 500, 505 to 888
        var groupName = array[0]; //Set groupName to 1.) Account Name
        var values = array[1].split(','); //Split 2.) 100 to 200, 500, 505 to 888 

        $('#inputGroupName').val(groupName.trim());
        $('.D-selection').closest('tr').remove();

        for (var i = 0; i < values.length; i++) {

            if (values[i].indexOf('to') > 0) { //After split, if string contain "to", append two input text with values
                var anotherArray = values[i].split('to');

                $('#add-more-condition').closest('tr').before('<tr><td><select class="D-selection">'
                                    + '<option value="equal">Equal to</option><option value="within" selected>Within range</option></select>'
                                    + '</td><td class="gd-return-section"><input type="text" value="' + anotherArray[0].trim() + '" /> To <input type="text" value="' + anotherArray[1].trim() + '" /><span class="gd-return-error-msg" style="display:none;"></span></td></tr>');

            }
            else { //if no, append one input text with value
                $('#add-more-condition').closest('tr').before('<tr><td><select class="D-selection">'
                                     + '<option value="equal" selected>Equal to</option><option value="within">Within range</option></select>'
                                     + '</td><td class="gd-return-section"><input type="text" value="' + values[i].trim() + '" /><span class="gd-return-error-msg" style="display:none;"></span></td></tr>');
            }
        }

        $('.D-selection:gt(0)').before('<span class="remove-condition-icon"><i class="fa fa-minus-circle"></i></span>'); //Add a remove icon for every select input except for the first child

        $('#gd-add-expression-btn').hide();
        $('#gd-update-section').show();
    });

    $('#rule-assignment').on('click', '#gd-update-expression-btn', function () {
        var str = "";
        var userView = "";
        var ready = true;
        var action = "update";
        var variableName = $('#pre-defined-variable-name').text();
        var groupName = $.trim($('#inputGroupName').val());

        if (groupName == "" || groupName == null) {
            $('#inputGroupName').next().remove();
            $('#inputGroupName').after('<span class="gd-return-error-msg">Please do not leave it blank.</span>');

            ready = false;
        }
        else {
            $('#inputGroupName').next().remove();
        }

        $('.D-selection').each(function () {
            if ($(this).val() == 'equal') {
                if ($.trim($(this).closest('tr').find('.gd-return-section input').first().val()) == "") {
                    $(this).closest('tr').find('.gd-return-section input').last().next().html('Please do not leave it blank.').show();

                    ready = false;
                }
                else {
                    $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                }
            }

            if ($(this).val() == 'within') {
                if ($.trim($(this).closest('tr').find('.gd-return-section input').first().val()) == "" || $.trim($(this).closest('tr').find('.gd-return-section input').last().val()) == "") {
                    $(this).closest('tr').find('.gd-return-section input').last().next().html('Please do not leave it blank.').show();

                    ready = false;
                }
                else {
                    $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                }

            }
        });

        if ($('#expression-list-div .exp-active').length == 0) {
            alert("Some problem occurs. Please cancel the update and start again.");

            ready = false;
            return false;
        }

        if ($('#expression-list-div .expression').length !== 0) {

            $('#expression-list-div .expression').each(function () {
                var EV_groupName = $(this).find('.expression-value').text();
                EV_groupName = EV_groupName.substring(EV_groupName.indexOf('"') + 1);
                EV_groupName = EV_groupName.substring(0, EV_groupName.indexOf('"'));

                var UV_groupName = $(this).find('.expression-userView').text();
                UV_groupName = UV_groupName.substring(0, UV_groupName.indexOf(':'));

                if (!$(this).hasClass('exp-active')) {
                    if (groupName.trim() == UV_groupName.trim()) {
                        $('#inputGroupName').after('<span class="gd-return-error-msg">Group name has been used. Please enter another.</span>');

                        ready = false;
                    }
                    else {
                        $(this).closest('tr').find('.gd-return-section input').last().next().hide();
                    }
                }
            });
        }

        //If all set
        if (ready) {

            str = str + "if ";
            str = str + "(";

            userView = userView + groupName + ' : ';

            $('.D-selection').each(function () {
                if ($(this).val() == 'equal') {
                    $(this).closest('tr').find('.gd-return-section input').first().next().hide();
                    str = str + '(' + variableName + ' == ' + $.trim($(this).closest('tr').find('.gd-return-section input').first().val()) + ') || ';

                    userView = userView + $.trim($(this).closest('tr').find('.gd-return-section input').first().val()) + ', ';
                }

                if ($(this).val() == 'within') {
                    $(this).closest('tr').find('.gd-return-section input').last().next().hide();

                    str = str + '((' + variableName + ' >= ' + $(this).closest('tr').find('.gd-return-section input').first().val() + ') && ';
                    str = str + '(' + variableName + ' <= ' + $(this).closest('tr').find('.gd-return-section input').last().val() + ')) || ';

                    userView = userView + $.trim($(this).closest('tr').find('.gd-return-section input').first().val());
                    userView = userView + " to " + $.trim($(this).closest('tr').find('.gd-return-section input').last().val()) + ', ';
                }
            });

            str = str.trim();
            str = str.substring(0, str.length - 2);
            str = str.trim();

            str = str + ')';
            str = str + ' return ' + '"' + groupName + '"' + ';';

            userView = userView.trim();
            userView = userView.substring(0, userView.length - 1);
            userView = userView.trim();

            group_ajax_parseStatement(action, userView, str, gd_resetD);
        }
    });

    $('#rule-assignment').on('click', '#gd-cancel-update-btn', function () {
        gd_resetD();
    });
}

function gd_resetE() {
    $('#inputCondition').empty();
    $('#inputCondition').attr('data-value', '');
    $('#gd-return-section select').val("");

    /*
    Move data-temp value to data-variable
    Split variables in the data-temp into array    
    Loop check each variable in the array is existed in the data-variable 
    if not existed, add in
    Lastly, collect all the variable in the data-variable and wrap it in the script and send to evaluate.
    */
    var variables = $('#inputCondition').attr('data-variable');
    var temp = $('#inputCondition').attr('data-temp');
    var split_temp = temp.split(",");

    for (var i = 0; i < split_temp.length; i++) {
        if (split_temp[i] !== "") {
            if (variables.indexOf(split_temp[i]) == -1) {
                variables = variables + split_temp[i] + ",";
            } 
        }
    }

    $('#inputCondition').attr('data-variable', variables);
    $('#inputCondition').attr('data-temp', '');   
}

//Reset setup group textfield, buttons and etc.
function gd_resetD() { 
    $('.D-selection').closest('tr').remove(); //Remove all select 
    $('#inputGroupName').val(""); //Set group name to empty
    $('#inputGroupName').closest('tr').after('<tr><td><select class="D-selection">'
                                     + '<option value="equal">Equal to</option><option value="within">Within range</option></select>'
                                     + '</td><td class="gd-return-section"><input type="text" /><span class="gd-return-error-msg" style="display:none;"></span></td></tr>'); //append one select with one text field (Back to default)

    $('#gd-add-expression-btn').show(); //Show add button
    $('#gd-update-section').hide(); //Hide update button
    $('#expression-list-div').find('.exp-editable').removeClass('exp-active'); //Remove active expression
    $('.gd-return-error-msg').remove(); //Remove all error message
}

//Special for Setup Account & Cheque group only
function group_ajax_parseStatement(action, userView, ruleStmt, resetFunction) {

    var parseMsg = null;
    var isSucceed = false;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/parseStatement",
        data: "{stmt : '" + ruleStmt + "'}",
        dataType: "json",
        success: function (data) {
            parseMsg = data.d;
            switch (parseMsg.STATUS) {
                case 0:

                    alert("Successful.");
                    isSucceed = true;

                    if (action == "add") {
                        $('#expression-list-div').append('<div class="expression exp-editable"><span class="remove-expression-icon"><i class="fa fa-trash-o"></i></span>'
                                                    + '<div class="expression-userView">' + userView + '</div><div class="expression-value" style="display:none;">' + ruleStmt + '</div>');
                    }
                    else if (action == "update") {
                        $('#expression-list-div > .exp-active').find('.expression-userView').html(userView);
                        $('#expression-list-div > .exp-active').find('.expression-value').html(ruleStmt);                        
                    }

                    resetFunction();

                    break;
                case 1:
                    alert(parseMsg.MESSAGE)
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
                case 2:
                    alert(parseMsg.MESSAGE);
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
                default:
                    alert(' Unknown Parse Error ');
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
            }
        },
        error: function (error) {
            alert("Error occurs when trying to parse.");
            $('#inputCondition').attr('data-temp', '');
        }
    });
    return isSucceed;
}

function gd_ajax_parseStatement(userView, ruleStmt, resetFunction) {

    var parseMsg = null;
    var isSucceed = false;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/parseStatement",
        data: "{stmt : '" + ruleStmt + "'}",
        dataType: "json",
        success: function (data) {
            parseMsg = data.d;
            switch (parseMsg.STATUS) {
                case 0:

                    alert("Successful.");
                    isSucceed = true;

                    $('#expression-list-div').append('<div class="expression"><span class="remove-expression-icon"><i class="fa fa-trash-o"></i></span>'
                                                    + '<div class="expression-userView">' + userView + '</div><div class="expression-value" style="display:none;">' + ruleStmt + '</div>');

                    resetFunction();

                    break;
                case 1:
                    alert(parseMsg.MESSAGE)
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
                case 2:
                    alert(parseMsg.MESSAGE);
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
                default:
                    alert(' Unknown Parse Error ');
                    isSucceed = false;
                    $('#inputCondition').attr('data-temp', '');
                    break;
            }
        },
        error: function (error) {
            alert("Error occurs when trying to parse.");
            $('#inputCondition').attr('data-temp', '');
        }
    });
    return isSucceed;
}

function gd_saveGlobalDefault() {
    $('#rule-assignment').on('click', '#save-gd-btn', function () {

        var result_type = $('#gd-return-section > div').attr('result-type');
        var ruleID = $('#gd-rule-id').text();
        var str = "";
        var expr = "";
        var userView = "";
        var variableName;

        switch (result_type) {
            case "I":

                var trim_GD_return = $.trim($('#inputGDReturn').val());
                var gd_value_min = parseInt($('#inputGDReturn').attr('min'));
                var gd_value_max = parseInt($('#inputGDReturn').attr('max'));

                if (trim_GD_return != "" && trim_GD_return.substr(0, 1) == '0') {
                    alert("Number cannot start with 0. Please re-enter again.");

                    return false;
                }

                if (trim_GD_return !== "") {
                    var isnum = /^[0-9]+$/.test(trim_GD_return); /* Validate if the input is not in [0-9] */

                    if (isnum == true) { /* If number is within [0-9], no negative */

                        gd_return_value = parseInt(trim_GD_return);

                        if ($('#inputGDReturn').attr('min') && $('#inputGDReturn').attr('max')) { /* Has min and max (Rule 1,5,7,8,9,14,15,16) */

                            if (gd_return_value >= gd_value_min && gd_return_value <= gd_value_max) { /* Check if the user input is greater than min and smaller than max */

                                $(this).attr('disabled', true); //temporary disable button when sending request to server

                                str = str + 'function()';
                                str = str + '{';
                                str = str + 'return ' + trim_GD_return;
                                str = str + ';}';

                                gd_ajax_parseScript(ruleID, trim_GD_return, trim_GD_return, str);
                            }
                            else {
                                alert("Please enter the number within the range. \n\nFor more information, please refer to the remark.");
                            }
                        }
                        else { /* Has no min and max (Rule 17,18) */

                            $(this).attr('disabled', true); //temporary disable button when sending request to server

                            str = str + 'function()';
                            str = str + '{';
                            str = str + 'return ' + trim_GD_return;
                            str = str + ';}';

                            gd_ajax_parseScript(ruleID, trim_GD_return, trim_GD_return, str);
                        }
                    }
                    else {
                        alert("Please enter positive number only.");
                    }
                }
                else {
                    $('#inputGDReturn').next().html('<i class="fa fa-times-circle"></i>Please do not leave it blank.').show();
                    $('#inputGDReturn').focus();
                }
                break;

            case "B":

                if ($('#gd-return-section .B-checkbox:checked').length == 1) {

                    var checked_value = $('.B-checkbox:checked').val().toLowerCase();

                    $(this).attr('disabled', true); //temporary disable button when sending request to server

                    str = str + 'function()';
                    str = str + '{';
                    str = str + 'return ' + checked_value;
                    str = str + ';}';

                    gd_ajax_parseScript(ruleID, checked_value, checked_value, str);
                }
                else {
                    $('.gd-return-error-msg').html('<i class="fa fa-times-circle"></i>Please choose either &#145;Yes&#146; or &#145;No&#146;').show();
                }
                break;

            case "E":

                variableName = $('#inputCondition').attr('data-variable');
                variableName = variableName.substring(0, variableName.length - 1);

                if ($('#expression-list-div .expression').length == 0) {
                    alert("No expression yet. Unable to proceed.");
                }
                else {

                    $(this).attr('disabled', true); //temporary disable button when sending request to server
                    
                    str = str + 'function(' + variableName + ')';
                    str = str + '{';

                    $('#expression-list-div .expression').each(function () {
                        expr = expr + $(this).find('.expression-value').text();

                        userView = userView + $(this).find('.expression-userView').text() + ';';
                    });

                    str = str + expr;
                    str = str + '}';

                    gd_ajax_parseScript(ruleID, userView, expr, str);
                }

                break;

            case "T":
                //Pending
                var time = $('#inputTime').val();

                if (time == "") {
                    alert("Please specify the time.");
                }
                else {

                    $(this).attr('disabled', true); //temporary disable button when sending request to server

                    str = str + 'function()';
                    str = str + '{';
                    str = str + 'return "' + time;
                    str = str + '";}';

                    gd_ajax_parseScript(ruleID, time, time, str);
                }
                break;

            case "D":
                var ready = true;

                variableName = $('#pre-defined-variable-name').text();

                if ($('#expression-list-div .expression').length == 0) {
                    alert("No expression yet. Unable to proceed.");
                }
                else {

                    //First check - Check duplicate group name
                    var groupNameList = document.getElementById("expression-list-div").getElementsByClassName("expression-userView");
                    var groupNameArray = [];

                    for (var a = 0; a < groupNameList.length; a++) {
                        var groupName = groupNameList[a].innerText;
                        groupName = groupName.substring(0, groupName.indexOf(':'));

                        groupNameArray.push(groupName.trim());
                    }

                    groupNameArray.sort();

                    for (var b = 1; b < groupNameArray.length; b++) {
                        if (groupNameArray[b - 1] == groupNameArray[b]) {
                            alert("Group name has duplicated. Please remove either one.");
                            ready = false;
                            break;
                        }
                        else {
                            continue;
                        }
                    }

                    /*
                    Second checking - Check if User View (UV) expression and the actual expression value (EV) is same
                    if UV and EV is not equal,
                    some problem has occurs. 
                    - It possible that user edit the HTML code by using Inspect Element (Chrome) or Firebug (Firefox).
                    */
                    $('#expression-list-div .expression').each(function () {
                        var EV = $(this).find('.expression-value').text();
                        EV = EV.substring(EV.indexOf('"') + 1);
                        EV = EV.substring(0, EV.indexOf('"'));

                        var UV = $(this).find('.expression-userView').text();
                        UV = UV.substring(0, UV.indexOf(':'));

                        if (EV.trim() !== UV.trim()) {
                            alert("Problem occurs. Please remove all the groups and add again.");
                            ready = false;
                            return false;
                        }
                    });
                }

                if (ready) {

                    $(this).attr('disabled', true); //temporary disable button when sending request to server

                    str = str + 'function(' + variableName + ')';
                    str = str + '{';

                    $('#expression-list-div .expression').each(function () {
                        expr = expr + $(this).find('.expression-value').text();

                        userView = userView + $(this).find('.expression-userView').text() + ';';
                    });

                    str = str + expr;

                    if (variableName == 'AccountNo') {
                        str = str + '}';
                    }

                    if (variableName == 'BankCode') {
                        str = str + 'return "LocalCheck";}';
                    }

                    gd_ajax_parseScript(ruleID, userView, expr, str);
                }

                break;
        }
    });
}

function gd_ajax_parseScript(ruleID, userRepresentative, expr, script) {

    var parseMsg = null;
    var isSucceed = false;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/parseScript",
        data: "{ruleID : '" + ruleID + "', userRepresentative : '" + userRepresentative + "', expression : '" + expr + "', script : '" + script + "'}",
        dataType: "json",
        success: function (data) {
            parseMsg = data.d;
            switch (parseMsg.STATUS) {
                case 0:
                    alert("Successful. Your action will be sent to checker for approval.");
                    isSucceed = true;

                    gd_GlobalSucceed();

                    break;
                case 1:
                    alert(parseMsg.MESSAGE)
                    isSucceed = false;
                    break;
                case 2:
                    alert(parseMsg.MESSAGE);
                    isSucceed = false;
                    break;
                default:
                    alert(' Unknown Parse Error ');
                    isSucceed = false;
                    break;
            }
        },
        error: function (error) {
            alert("Error occurs when trying to parse script.");
        }
    });
    return isSucceed;
}

function gd_GlobalSucceed() {
    $('#global-default-rule-modal').remove();
    global_default_modal_opened = false;
    $('#modal-bg-opacity').remove();
    $('body').css('overflow', 'auto');

    $('#rule-assignment-table > tbody').empty();
    fn_getRulesList();
}

function gd_GlobalDefault() {
    //Close modal
    $('#rule-assignment').on('click', '#global-default-rule-bg, #close-gd-modal, #close-gd-btn', function () {
        global_default_modal_opened = false; //Reset open global default flag

        $('#modal-bg-opacity').remove();
        $('#global-default-rule-modal').remove();
        $('body').css('overflow', 'auto');

        //Reset the variable operator
        variable_operator_div_opened = false;
        $('#variable-operator-popup').remove();
    });

    //Global Default keyup validation
    $('#rule-assignment').on('keyup', '#inputGDReturn', function () {

        var trim_GD_return = $.trim($(this).val());
        var gd_return_value = parseInt(trim_GD_return);
        var gd_value_min = parseInt($('#inputGDReturn').attr('min'));
        var gd_value_max = parseInt($('#inputGDReturn').attr('max'));


        if (trim_GD_return != "" && trim_GD_return.substr(0, 1) == '0') {
            $(this).next().html('<i class="fa fa-times-circle"></i>Number cannot start with 0. Please re-enter again.').fadeIn('fast');

            return false;
        }

        if (trim_GD_return !== "") {
            var isnum = /^[0-9]+$/.test(trim_GD_return);

            if (isnum == true) {

                if ($('#inputGDReturn').attr('min') && $('#inputGDReturn').attr('max')) {

                    if (!(gd_return_value >= gd_value_min && gd_return_value <= gd_value_max)) {
                        $(this).next().html('<i class="fa fa-times-circle"></i>Number out of range.').fadeIn('fast');
                    }
                    else {
                        $(this).next().hide();
                    }
                }
                else {
                    $(this).next().hide();
                }
            }
            else {
                $(this).next().html('<i class="fa fa-times-circle"></i>Invalid number.').fadeIn('fast');
            }
        }
        else {
            $(this).next().html('<i class="fa fa-times-circle"></i>Please do not leave it blank.').fadeIn('fast');
        }
    });

    //Validation on checkbox + checkbox toggle
    $('#rule-assignment').on('click', '#gd-return-section .B-checkbox', function () {
        if ($('#gd-return-error-msg').is(':visible')) {
            $('#gd-return-error-msg').fadeOut('fast');
        }
        $('#gd-return-section').find('.B-checkbox:checked').prop('checked', false);
        $(this).prop('checked', true);
    });

    //On select, change the input type
    $('#rule-assignment').on('change', '.D-selection', function () {
        if ($(this).find(':selected').val() == 'equal') {
            $(this).closest('tr').find('.gd-return-section').html('<input type="text" /><span class="gd-return-error-msg" style="display:none;"></span>');
        }
        else {
            $(this).closest('tr').find('.gd-return-section').html('<input type="text" /> To <input type="text" /><span class="gd-return-error-msg" style="display:none;"></span>');
        }
    });

    //On click, add another condition
    $('#rule-assignment').on('click', '#add-more-condition', function () {
        $(this).closest('tr').before('<tr><td><span class="remove-condition-icon"><i class="fa fa-minus-circle"></i></span><select class="D-selection">'
                                     + '<option value="equal">Equal to</option><option value="within">Within range</option></select>'
                                     + '</td><td class="gd-return-section"><input type="text" /><span class="gd-return-error-msg" style="display:none;"></span></td></tr>');
    });

    //On click, remove condition
    $('#rule-assignment').on('click', '.remove-condition-icon', function () {
        $(this).closest('tr').fadeOut('fast', function () {
            $(this).remove();
        });
    });

    $('#rule-assignment').on('mouseover', '.setup-group-name', function () {
        var expr = $(this).find('.setup-group-expr').text();

        $(this).append('<div id="expr-tooltip">' + expr + '</div>');

    }).mouseout(function () {
        $('#expr-tooltip').remove();
    }); 
}

function fn_removeExpression() {
    //On click, delete expression
    $('#rule-assignment').on('click', '.remove-expression-icon', function (event) {
        event.stopPropagation()

        var confirmed = confirm("Are you sure you want to remove?");

        if (confirmed) {
            if ($(this).closest('.expression').hasClass('exp-active')) {
                gd_resetD();
            }

            $(this).closest('.expression').fadeOut('fast', function () {
                $(this).remove();
            });
        }
    });
}

function sr_loadSpecificValueConfig() {
    $('#rule-assignment').on('click', '.specific-rule-column > a', function () {
        var str = "";
        var str1 = "";
        var ruleID = $(this).closest('tr').find('td > .global-default-rule').attr('id');

        str = str + '<div id="modal-bg-opacity"></div>';
        str = str + '<div id="specific-rule-modal">';
        str = str + '<div id="specific-rule-bg"></div>';
        str = str + '<div id="machine-list-main-div">';
        str = str + '<table><tbody><tr style="vertical-align:top;">';
        str = str + '<td><div id="gd-value-div">';
        str = str + '<div id="gd-current-value">'
        str = str + '<span id="close-sr-div"><i class="fa fa-times"></i></span>';
        str = str + '<div id="gd-current-rule-id" style="display:none;"></div>'
        str = str + '<div id="gd-current-rule-name"></div>'
        str = str + '<div id="gd-current-rule-value"></div>'
        str = str + '</div></div>';
        str = str + '<div id="selected-machine-div">';
        str = str + '<span style="font-weight: bold; font-size: 14px;">You have selected:</span>';
        str = str + '<div id="selected-machine-list"><ul id="selected-machine"></ul></div></div></td>';

        //Another TD
        //Div 1 (select machine)
        str = str + '<td><div id="machine-list-div">';
        str = str + '<div id="machine-list-header">';
        str = str + '<h4>Please select machine ID</h4><hr />';
        str = str + '<div id="machine-list-search">';
        str = str + '<i class="fa fa-search"></i><input type="text" id="inputSearchMachine" />'
        str = str + '</div></div>';
        str = str + '<div id="machine-list-body"><div class="row"></div></div><hr />';
        str = str + '<div id="machine-list-footer">';
        str = str + '<button class="btn btn-default" id="close-sr-btn">Close</button>&nbsp;';
        str = str + '<button class="btn btn-primary" id="continue-btn" style="display:none;">Continue</button>';
        str = str + '</div></div>';

        //Div 2 (Specific value config)
        str = str + '<div id="sr-value-div" style="display:none;">';
        str = str + '</div>';

        //end
        str = str + '</td></tr></tbody></table></div>';

        str = str + '</div>';

        $('body').css('overflow', 'hidden');
        $('#rule-assignment').append(str);

        /*
        Get Global Default information
        */
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getGlobalDefault",
            data: "{ruleID : '" + ruleID + "'}",
            dataType: "json",
            success: function (data) {
                for (var i = 0; i < data.d.length; i++) {
                    $('#gd-current-rule-id').html(data.d[i].RuleID);
                    $('#gd-current-rule-name').html(data.d[i].RuleName);
                    $('#gd-current-rule-value').html(data.d[i].UserViewExpr);
                }
            },
            error: function (error) {
                alert("Error occurs when trying to load the global default.");
            }
        });

        /*
        Get all machines
        */
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getMachineList",
            data: "{ruleID : '" + ruleID + "'}",
            dataType: "json",
            beforeSend: function () {
                $('#machine-list-body > .row').html('Loading...');
            },
            success: function (data) {

                var machines = "";

                for (var i = 0; i < data.d.length; i++) {
                    machines = machines + '<div class="col-md-2 a-machine" branch-code="' + data.d[i].BranchCode + '" branch-name="' + data.d[i].BranchName + '" '
                                                        + 'address1="' + data.d[i].Address1 + '" address2 = "' + data.d[i].Address2 + '">' + data.d[i].MachineID + '</div>';
                }

                $('#machine-list-body > .row').html(machines);
            },
            error: function (error) {
                alert("Error occurs when trying to load the machine list.");
            }
        });

        /*
        Get rule detail (Input type and etc)
        */
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/rulesAssignment/main.aspx/getRuleDetail",
            data: "{ruleID : '" + ruleID + "'}",
            dataType: "json",
            success: function (data) {
                str1 = str1 + '<div id="sr-value-header">';
                str1 = str1 + '<p class="config-rule-label">' + data.d[0].RuleLabel + '</p></div>';
                str1 = str1 + '<div id="sr-value-body">';
                str1 = str1 + '<table><tbody>';

                var isCondtion = (data.d[0].IsCondition).toUpperCase();

                if (isCondtion == 'TRUE') {
                    str1 = str1 + '<tr id="sr-condition"><td><label>Condition</label></td>';
                    str1 = str1 + '<td><div id="inputCondition" data-value="" data-variable="" data-temp=""></div></td></tr>';

                }

                var resultType = (data.d[0].ResultType).toUpperCase();

                switch (resultType) {
                    case "I":

                        str1 = str1 + '<tr><td><label id="sr-return-label"><label></td>';
                        str1 = str1 + '<td id="sr-return-section">';
                        str1 = str1 + '<div result-type="I"><input type="text" id="inputSRReturn" min="' + data.d[0].IntegerMin + '" max="' + data.d[0].IntegerMax + '" result-type="I" /><span class="sr-return-error-msg" style="display:none;"></span></div>';
                        str1 = str1 + '</td></tr>';

                        break;

                    case "B":

                        str1 = str1 + '<tr><td><label id="sr-return-label"><label></td>';
                        str1 = str1 + '<td id="sr-return-section">';
                        str1 = str1 + '<div result-type="B"><input type="checkbox" class="B-checkbox" value="true" /><label>Yes</label><input type="checkbox" class="B-checkbox" value="false" /><label>No</label><span class="sr-return-error-msg" style="display:none;"></span></div>';
                        str1 = str1 + '</td></tr>';

                        break;

                    case "E":
                        str1 = str1 + '<tr><td><label id="gd-return-label">Go To<label></td>';
                        str1 = str1 + '<td id="sr-return-section">';
                        str1 = str1 + '<div result-type="E"><select><option value="1">Bin 1</option><option value="2">Bin 2</option><option value="3">Bin 3</option><option value="4">Bin 4</option></select></div>';
                        str1 = str1 + '</td></tr>';
                        str1 = str1 + '<tr><td></td><td><button class="btn btn-primary" id="sr-add-expression-btn">Add</button></td></tr>';
                        str1 = str1 + '<tr id="expression-list-row"><td><label>Expression</label></td><td><div id="expression-list-div"></div></td></tr>';

                        break;

                    case "T":

                        str1 = str1 + '<tr><td><label id="sr-return-label"><label></td>';
                        str1 = str1 + '<td id="sr-return-section">';
                        str1 = str1 + '<div result-type="T"><input id="inputTime" type="time" /><span class="gd-return-error-msg" style="display:none;"></span></div>';
                        str1 = str1 + '</td></tr>';

                        break;
                }

                str1 = str1 + '<tr><td></td><td><div class="remark-section"><span class="rule-remark">' + data.d[0].RuleRemark + '</span></div></td></tr>';
                str1 = str1 + '<tr><td></td><td><button type="button" class="btn btn-default" id="back-select-machine-btn">Back</button>';
                str1 = str1 + '&nbsp;<button type="button" class="btn btn-primary" id="save-sr-btn">Save</button></td></tbody></table>';
                str1 = str1 + '</div>';

                $('#sr-value-div').html(str1);
            },
            error: function (error) {
                alert("Error occurs when trying to load the machine list.");
            }
        });
    });
}

function sr_searchMachine() {
    $('#rule-assignment').on('keyup', '#inputSearchMachine', function () {

        var ruleID = $('#gd-current-rule-id').text();
        var searchPattern = $(this).val();
        var machines = "";
        var selectedMachines = document.getElementById("selected-machine").getElementsByTagName("li");

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/rulesAssignment/main.aspx/searchMachine",
            data: "{ruleID : '" + ruleID + "', searchPattern: '" + searchPattern + "'}",
            dataType: "json",
            success: function (data) {

                var machines = "";

                for (var i = 0; i < data.d.length; i++) {
                    machines = machines + '<div class="col-md-2 a-machine" branch-code="' + data.d[i].BranchCode + '" branch-name="' + data.d[i].BranchName + '" '
                                        + 'address1="' + data.d[i].Address1 + '" address2 = "' + data.d[i].Address2 + '">' + data.d[i].MachineID + '</div>';
                }

                $('#machine-list-body > .row').html(machines);

                var machineList = document.getElementById("machine-list-body").getElementsByClassName("a-machine");

                for (var j = 0; j < machineList.length; j++) {
                    for (var k = 0; k < selectedMachines.length; k++) {
                        if (selectedMachines[k].innerText == machineList[j].innerText) {
                            machineList[j].className = machineList[j].className + " a-selected-machine";
                            continue;
                        }
                    }
                }

            },
            error: function (error) {
                alert("Error occurs when trying to load the machine list.");
            }
        });
    });
}

function sr_addExpression() {

    $('#rule-assignment').on('click', '#sr-add-expression-btn', function () {

        var result_type = $('#sr-return-section > div').attr('result-type');
        var str = "";
        var ready = true;

        switch (result_type.toUpperCase()) {
            case "E":

                var inputConditionValue = $('#inputCondition').attr('data-value');
                var setTo = $('#sr-return-section select').val();

                var conditionText = $('#inputCondition').text();
                var goTo = $('#sr-return-section option:selected').text();
                var userView = "";

                if ($('#inputCondition').text() == "") {
                    alert("Please input condition");

                    return false;
                }

                if (setTo !== null) {
                    str = str + 'if (' + inputConditionValue + ')) ';
                    str = str + 'return \"' + setTo + '\";';

                    userView = userView + conditionText + ', Go to "' + goTo + '"';

                    gd_ajax_parseStatement(userView, str, sr_resetE);
                }
                else {
                    alert("Please choose");
                    $('#sr-return-section select').focus();
                }

                break;
        }
    });
}

function sr_resetE() {
    $('#inputCondition').empty();
    $('#inputCondition').attr('data-value', '');
    $('#sr-return-section select').val("");

    /*
    Move data-temp value to data-variable
    Split variables in the data-temp into array    
    Loop check each variable in the array is existed in the data-variable 
    if not existed, add in
    Lastly, collect all the variable in the data-variable and wrap it in the script and send to evaluate.
    */
    var variables = $('#inputCondition').attr('data-variable');
    var temp = $('#inputCondition').attr('data-temp');
    var split_temp = temp.split(",");

    for (var i = 0; i < split_temp.length; i++) {
        if (split_temp[i] !== "") {
            if (variables.indexOf(split_temp[i]) == -1) {
                variables = variables + split_temp[i] + ",";
            }
        }
    }

    $('#inputCondition').attr('data-variable', variables);
    $('#inputCondition').attr('data-temp', '');
}

function sr_ajax_parseScript(machine_list, ruleID, userRepresentative, expr, script) {

    var parseMsg = null;
    var isSucceed = false;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/parseScript_Specific",
        data: JSON.stringify({ machineList: machine_list, ruleID: ruleID, userRepresentative: userRepresentative, expression: expr, script: script }),
        dataType: "json",
        success: function (data) {
            parseMsg = data.d;
            switch (parseMsg.STATUS) {
                case 0:
                    alert("Successful. Your action will be sent to checker for approval.");
                    isSucceed = true;

                    sr_SpecificSucceed();

                    break;
                case 1:
                    alert(parseMsg.MESSAGE)
                    isSucceed = false;
                    break;
                case 2:
                    alert(parseMsg.MESSAGE);
                    isSucceed = false;
                    break;
                default:
                    alert(' Unknown Parse Error ');
                    isSucceed = false;
                    break;
            }
        },
        error: function (error) {
            alert("Error occurs when trying to parse script.");
        }
    });
    return isSucceed;
}

function sr_saveSpecificRule() {

    $('#rule-assignment').on('click', '#save-sr-btn', function () {

        var result_type = $('#sr-return-section > div').attr('result-type');
        var ruleID = $('#gd-current-rule-id').text();
        var machine_list = [];
        var str = "";

        $('#machine-list-body .a-selected-machine').each(function () {
            machine_list.push($(this).text());
        });

        switch (result_type) {
            case "I":

                var trim_SR_return = $.trim($('#inputSRReturn').val());
                var sr_value_min = parseInt($('#inputSRReturn').attr('min'));
                var sr_value_max = parseInt($('#inputSRReturn').attr('max'));

                if (trim_SR_return != "" && trim_SR_return.substr(0, 1) == '0') {
                    alert("Number cannot start with 0. Please re-enter again.");

                    return false;
                }

                if (trim_SR_return !== "") {
                    var isnum = /^[0-9]+$/.test(trim_SR_return); /* Validate if the input is not in [0-9] */

                    if (isnum == true) { /* If number is within [0-9], no negative */

                        sr_return_value = parseInt(trim_SR_return);

                        if ($('#inputSRReturn').attr('min') && $('#inputSRReturn').attr('max')) { /* Has min and max (Rule 1,5,7,8,9,14,15,16) */

                            if (sr_return_value >= sr_value_min && sr_return_value <= sr_value_max) { /* Check if the user input is greater than min and smaller than max */
                                //ajax post to backend and insert into database

                                $.ajax({
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    url: "/views/kioskMaintenance/rulesAssignment/main.aspx/validateSpecificValue",
                                    data: JSON.stringify({ ruleID: ruleID, ruleExpression: trim_SR_return }),
                                    dataType: "json",
                                    success: function (data) {

                                        if (data.d) {

                                            $('#save-sr-btn').attr('disabled', true); //temporary disable button when sending request to server

                                            str = str + 'function()';
                                            str = str + '{';
                                            str = str + 'return ' + trim_SR_return;
                                            str = str + ';}';

                                            sr_ajax_parseScript(machine_list, ruleID, trim_SR_return, trim_SR_return, str);
                                        }
                                        else {
                                            alert("The entered value cannot be same as global default value. \n\nPlease try again.");
                                        }
                                    },
                                    error: function (error) {
                                        alert("Error occurs when trying to validate specific value.");
                                    }
                                });
                            }
                            else {
                                alert("Please enter the number within the range. \n\nFor more information, please refer to the remark.");
                            }
                        }
                        else { /* Has no min and max (Rule 17,18) */
                            //ajax post to backend and insert into database
                            alert("Im here 1");
                        }
                    }
                    else {
                        alert("Please enter positive number only.");
                    }
                }
                else {
                    $('#inputSRReturn').next().html('<i class="fa fa-times-circle"></i>Please do not leave it blank.').show();
                    $('#inputSRReturn').focus();
                }

                break;

            case "B":
                if ($('#sr-return-section .B-checkbox:checked').length == 1) {

                    var checked_value = $('.B-checkbox:checked').val().toLowerCase();

                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/validateSpecificValue",
                        data: JSON.stringify({ ruleID: ruleID, ruleExpression: checked_value }),
                        dataType: "json",
                        success: function (data) {

                            if (data.d) {

                                $('#save-sr-btn').attr('disabled', true); //temporary disable button when sending request to server

                                str = str + 'function()';
                                str = str + '{';
                                str = str + 'return ' + checked_value;
                                str = str + ';}';

                                sr_ajax_parseScript(machine_list, ruleID, checked_value, checked_value, str);
                            }
                            else {
                                alert("The entered value cannot be same as global default value. \n\nPlease try again.");
                            }
                        },
                        error: function (error) {
                            alert("Error occurs when trying to validate specific value.");
                        }
                    });
                }
                else {
                    $('.sr-return-error-msg').html('<i class="fa fa-times-circle"></i>Please choose either &#145;Yes&#146; or &#145;No&#146;').show();
                }
                break;

            case "E":
                var variableName = $('#inputCondition').attr('data-variable');
                variableName = variableName.substring(0, variableName.length - 1);
                var expr = "";
                var userView = "";

                if ($('#expression-list-div .expression').length == 0) {
                    alert("No expression yet. Unable to proceed.");
                }
                else {

                    $('#expression-list-div .expression').each(function () {
                        expr = expr + $(this).find('.expression-value').text();

                        userView = userView + $(this).find('.expression-userView').text() + ';';
                    });

                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/validateSpecificValue",
                        data: JSON.stringify({ ruleID: ruleID, ruleExpression: expr }),
                        dataType: "json",
                        success: function (data) {

                            if (data.d) {

                                $('#save-sr-btn').attr('disabled', true); //temporary disable button when sending request to server

                                str = str + 'function(' + variableName + ')';
                                str = str + '{';
                                str = str + expr;
                                str = str + '}';

                                sr_ajax_parseScript(machine_list, ruleID, userView, expr, str);
                            }
                            else {
                                alert("The entered value cannot be same as global default value. \n\nPlease try again.");
                            }
                        },
                        error: function (error) {
                            alert("Error occurs when trying to validate specific value.");
                        }
                    });
                }

                break;

            case "T":
                var time = $('#inputTime').val();

                if (time == "") {
                    alert("Please specify the time.");
                }
                else {

                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "/views/kioskMaintenance/rulesAssignment/main.aspx/validateSpecificValue",
                        data: JSON.stringify({ ruleID: ruleID, ruleExpression: time }),
                        dataType: "json",
                        success: function (data) {

                            if (data.d) {

                                $('#save-sr-btn').attr('disabled', true); //temporary disable button when sending request to server

                                str = str + 'function()';
                                str = str + '{';
                                str = str + 'return "' + time;
                                str = str + '";}';

                                sr_ajax_parseScript(machine_list, ruleID, time, time, str);
                            }
                            else {
                                alert("The entered value cannot be same as global default value. \n\nPlease try again.");
                            }
                        },
                        error: function (error) {
                            alert("Error occurs when trying to validate specific value.");
                        }
                    });
                }
                break;
        }
    });
}

function sr_SpecificSucceed() {
    $('#specific-rule-modal').remove();

    $('#modal-bg-opacity').remove();
    $('body').css('overflow', 'auto');

    $('#rule-assignment-table > tbody').empty();
    fn_getRulesList();
}

function sr_removeSpecificRule() {
    $('#rule-assignment').on('click', '.delete-gd-specific-icon', function () {
        var machines = $(this).closest('.gd-specific').find('.gd-specific-machine').text();
        var ruleID = $(this).closest('tr').find('td > .global-default-rule').attr('id');

        var confirmed = confirm("Are you sure you want to delete?");

        if (confirmed) {

            var machine_list = machines.split(",");

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/rulesAssignment/main.aspx/deleteSpecificRule",
                data: JSON.stringify({ machineList: machine_list, ruleID: ruleID }),
                dataType: "json",
                success: function (data) {

                    if (data.d) {
                        alert('Successful. Your action will be sent to checker for approval.');
                        sr_SpecificSucceed();
                    }
                    else {
                        alert('Failed! Unable to delete the specific rule.');
                    }
                },
                error: function (error) {
                    alert("Error occurs when trying to delete the specific rule.");
                }
            });
        }
    });
}

function sr_SpecificRule() {
    //Close Div
    $('#rule-assignment').on('click', '#close-sr-div, #close-sr-btn', function () {
        $('#modal-bg-opacity').remove();
        $('#specific-rule-modal').remove();

        $('body').css('overflow', 'auto');
    })

    /*
      Hover on the MachineID, show the detail
    */
    $('#rule-assignment').on('mouseover', '#machine-list-body .a-machine', function (e) {
        $('#machine-detail-tooltip').remove();
        var x = e.clientX;
        var y = e.clientY;

        var branchCode = $(this).attr('branch-code');
        var branchName = $(this).attr('branch-name');
        var address1 = $(this).attr('address1');
        var address2 = $(this).attr('address2');

        $('#specific-rule-modal').append('<div id="machine-detail-tooltip"><span class="machine-detail-branch-code">' + branchCode + '</span><span class="machine-detail-branch-name">' + branchName + '</span><br/>'
                                        +'<span class="machine-detail-address">' + address1 + '<br/>' + address2 + '</span></div>');
        $('#machine-detail-tooltip').css({ 'top': y, 'left': x });

    }).mouseout(function () {
        $('#machine-detail-tooltip').remove();
    });

    /*
      Click on machine ID, the selected machine ID will active
      The selected machine will display on the #selected-machine UL
      If no machine ID was seleceted, the continue button will hide
    */
    $('#rule-assignment').on('click', '#machine-list-body .a-machine', function () {

        var selectedMachines = document.getElementById("selected-machine").getElementsByTagName("li");
        var selected = false;

        if (selectedMachines.length !== 0) {
            for (var i = 0; i < selectedMachines.length; i++) {
                if ($(this).text() == selectedMachines[i].innerText) {
                    selectedMachines[i].remove();

                    selected = true;
                    break;
                }
            }

            if (selected) {
                $(this).removeClass('a-selected-machine');
            }
            else {
                $(this).addClass('a-selected-machine');
                $('#selected-machine').append("<li>" + $(this).text() + "</li>");
            }
        }
        else {
            $(this).addClass('a-selected-machine');
            $('#selected-machine').append("<li>" + $(this).text() + "</li>");
        }

        if ($('#machine-list-body .a-selected-machine').length > 0) {
            $('#continue-btn').fadeIn('fast');
        }
        else
            $('#continue-btn').hide();
    });

    //Proceede to specific rule setting
    $('#rule-assignment').on('click', '#continue-btn', function () {
        $('#machine-list-div').hide();
        $('#sr-value-div').show();
    });

    //Go back to select machine screen
    $('#rule-assignment').on('click', '#back-select-machine-btn', function () {
        $('#machine-list-div').show();
        $('#sr-value-div').hide();
    });

    //Specific rule keyup validation
    $('#rule-assignment').on('keyup', '#inputSRReturn', function () {

        var trim_SR_return = $.trim($(this).val());
        var sr_return_value = parseInt(trim_SR_return);
        var sr_value_min = parseInt($('#inputSRReturn').attr('min'));
        var sr_value_max = parseInt($('#inputSRReturn').attr('max'));


        if (trim_SR_return != "" && trim_SR_return.substr(0, 1) == '0') {
            $(this).next().html('<i class="fa fa-times-circle"></i>Number cannot start with 0. Please re-enter again.').fadeIn('fast');

            return false;
        }

        if (trim_SR_return !== "") {
            var isnum = /^[0-9]+$/.test(trim_SR_return);

            if (isnum == true) {

                if ($('#inputSRReturn').attr('min') && $('#inputSRReturn').attr('max')) {

                    if (!(sr_return_value >= sr_value_min && sr_return_value <= sr_value_max)) {
                        $(this).next().html('<i class="fa fa-times-circle"></i>Number out of range.').fadeIn('fast');
                    }
                    else {
                        $(this).next().hide();
                    }
                }
                else {
                    $(this).next().hide();
                }
            }
            else {
                $(this).next().html('<i class="fa fa-times-circle"></i>Invalid number.').fadeIn('fast');
            }
        }
        else {
            $(this).next().html('<i class="fa fa-times-circle"></i>Please do not leave it blank.').fadeIn('fast');
        }
    });

    $('#rule-assignment').on('click', '#sr-return-section .B-checkbox', function () {
        if ($('.sr-return-error-msg').is(':visible')) {
            $('.sr-return-error-msg').fadeOut('fast');
        }
        $('#sr-return-section').find('.B-checkbox:checked').prop('checked', false);
        $(this).prop('checked', true);
    });
}

function off_eventHandler() {
    $('#rule-assignment').off();
}

$(document).ready(function () {

    /*
    Shared
    */
    off_eventHandler(); //Off all event handlers when document is loaded
    fn_getRulesList(); //Get All Rule List (Global Default + Specific)
    fn_VariableOperator(); //List of Variable Operator functions (Global Default & Specific shared this function);
    fn_removeExpression();

    /*
    Global Default
    */
    gd_getRuleDetail(); //Get individual global default rule detail
    gd_addExpression(); //Global Default - Add expression
    group_editable(); //Special for setup Account & Cheque group
    gd_saveGlobalDefault(); //Save global default rule into database
    gd_GlobalDefault(); //Group all other global default functions (Close div, validation & etc)

    /*
    Specific 
    */
    sr_loadSpecificValueConfig(); //Load specific rule configuration, get machine ID from database, get rule detail
    sr_searchMachine(); //Search machines by enter machine id
    sr_addExpression(); //Specific - Add expression
    sr_saveSpecificRule(); //Save specific rule into database
    sr_removeSpecificRule(); //Remove specific rule from database
    sr_SpecificRule(); //Group all other specific rule functions
}); 