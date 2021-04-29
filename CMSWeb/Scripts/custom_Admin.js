/**
  Load ajax content
  @param String URL
  @param Fnuction callback
  @param String jContainerSelector container -> default is "#gridcontainer"
  @param Boolean isReplaceUrl replace URL -> default true
 */
X.Page.getAjaxList = function (url, callback, jContainerSelector, isReplaceUrl) {
    isReplaceUrl = X.F.is(isReplaceUrl, "undefined") ? true : isReplaceUrl;

    X.A.xhr(url, !1)
        .done(function (data, textStatus, xhr) {
            // Check session
            X.F.checkSession(xhr);

            jContainerSelector = X.F.is(jContainerSelector, "undefined") ? "#gridcontainer" : jContainerSelector;
            $(jContainerSelector).html(data);

            (X.F.isHistory() && isReplaceUrl) ? history.replaceState(null, "", url) : "";

            //callback
            X.F.is(callback, "function") ? callback() : "";
        });
}

/**
  Enable-Disable confirm box
  Just need ID + Token
 */
X.Page.setEnableDisableConfirm = function () {
    // Yes button
    $("#popbox button.yes").off().on("click", function () { // Form submit
        $("#popbox form").submit(function () {
            let frm = $(this)
                , token = $("[name=__RequestVerificationToken]", frm).val()
                , id = parseInt($.trim($("[name=Id]", frm).val()));

            if (isNaN(id) || id < 1 || token === "") {
                X.F.setError("Invalid Id", !1, !0);
                return !1;
            }

            // Begin submit with Loading Box
            X.A.xhr(frm.prop("action"), !0,
                {
                    id: id,
                    __RequestVerificationToken: token
                },
                function () {
                    X.F.setError("", !0);
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data)
                        , message = data.message
                        , code = parseInt(data.code);

                    // Success - message will be URL to reload
                    if (code === 1) {
                        X.Thikbox.remove();
                        X.F.isHistory()
                            ? X.Page.getAjaxList(location.href, function () { X.F.doEffect($("#" + message.split("#").pop())) })
                            : location.reload();
                    }
                    else {
                        X.Thikbox.remove();
                        X.Thikbox.load(message);
                    }
                })

            return !1;
        });
    });
}


/**
 * Get all location form API
 * https://thongtindoanhnghiep.co/rest-api
 * */
X.Page.getAPILocation = function (LocationJson) {


}

X.Page.User =
{
    search_user: function (jGridContainerSelector) {
        let frm = $("#user_search_form")
            , btnSbm = $("[type=submit]", frm)
            , query = $.trim($("[name=query]", frm).val())
            , status = $.trim($("[name=status]", frm).val())
            , urlQuery = location.pathname + "?"
            , urlParams = {};

        (query !== "") ? urlParams.query = X.F.doKeywordFilter(query) : "";
        (parseInt(status) >= 0) ? urlParams.status = status : "";

        urlQuery += $.param(urlParams);
        $("[name=query]", frm).val(urlParams.query);

        X.A.xhr(urlQuery, !1, "",
            function () {
                btnSbm.prop("disabled", true);
            })
            .done(function (data, textStatus, xhr) {
                // Check session               
                X.F.checkSession(xhr);

                // Success return HTML grid
                jGridContainerSelector = jGridContainerSelector || "#gridcontainer";
                $(jGridContainerSelector).html(data);
                X.F.isHistory() ? history.replaceState("", "", urlQuery) : "";
                btnSbm.prop("disabled", false);
            });

        return !1;
    },

    set_role: function () {
        $("#popbox form").submit(function () {
            let frm = $(this)
                , token = $("[name=__RequestVerificationToken]", frm).val()
                , id = parseInt($.trim($("[name=Id]", frm).val()))
                , role = $("[name=Roles]", frm).val()
                , btnSbm = $("[type=submit]", frm);

            if (isNaN(id)) {
                alert("Error! Employee id");
                return !1;
            }

            if (token === "") {
                X.F.setError("Error token!", !1);
                return !1;
            }

            // Begin submit
            X.A.xhr(frm.prop("action"), !0,
                {
                    id: id,
                    role: role,
                    __RequestVerificationToken: token
                }, function () {
                    X.F.setError("", !0);
                    btnSbm.prop("disabled", !0).addClass("disabled");
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload

                    if (code == 1) {
                        X.Thikbox.remove();
                        X.Page.getAjaxList(window.location.href, function () { X.F.doEffect($("#" + message.split("#").pop())) })
                    }
                    else {
                        X.F.setError(message, !1);
                        btnSbm.prop("disabled", !1).removeClass("disabled");
                    }
                })
            return !1;
        });
    },

    init: function () {
        let that = this;

        $("#user_search_form").off().on("submit", function () {
            that.search_user("#gridcontainer");
        });

        /*--- 2. Disable + Enable buttons */
        $("[data-action=disable], [data-action=enable]").off().on("click", function (e) {
            e.preventDefault();

            // Load confirm popbox
            let id = parseInt($(this).data("id")),
                action = $(this).data("action");
            if (isNaN(id) || action == "") {
                alert("Invalid action id");
                return !1;
            }

            let title = action === "disable" ? "Disable" : "Enable";

            X.A.xhr("/user/" + action + "/" + id)
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, title, function () {
                        X.Page.setEnableDisableConfirm()
                    });

                });
        });

        /*--- 3. Set role buttons */
        $("[data-action=setrole]").off().on("click", function (e) {
            e.preventDefault();

            // Load confirm popbox
            let id = parseInt($(this).data("id"));

            if (isNaN(id)) {
                alert("Invalid action id");
                return !1;
            }

            let title = "Set role";

            X.A.xhr("/user/SetRole/" + id)
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, title, function () {
                        X.Page.User.set_role();
                    });

                });
        });
    },
}

X.Page.Employee =
{
    search_employee: function (jGridContainerSelector, sortColumnName) {
        let frm = $("#employee_search_form")
            , btnSbm = $("[type=submit]", frm)
            , query = $.trim($("[name=query]", frm).val())
            , status = $.trim($("[name=status]", frm).val())
            , urlQuery = location.pathname + "?"
            , urlParams = {};

        (query !== "") ? urlParams.query = X.F.doKeywordFilter(query) : "";
        (parseInt(status) >= 0) ? urlParams.status = status : "";
        (sortColumnName !== "") ? urlParams.sortColumnName = sortColumnName : "";

        urlQuery += $.param(urlParams);
        $("[name=query]", frm).val(urlParams.query);

        X.A.xhr(urlQuery, !1, "",
            function () {
                btnSbm.prop("disabled", true);
            })
            .done(function (data, textStatus, xhr) {
                // Check session               
                X.F.checkSession(xhr);

                // Success return HTML grid
                jGridContainerSelector = jGridContainerSelector || "#gridcontainer";
                $(jGridContainerSelector).html(data);
                X.F.isHistory() ? history.replaceState("", "", urlQuery) : "";
                btnSbm.prop("disabled", false);
            });

        return !1;
    },

    sort_employee: function () {
        $("[data-action=sortemployee]").off().on("click", function (e) {
            e.preventDefault();
            let sortColumnName = $(this).data("field");

            X.Page.Employee.search_employee("#gridcontainer", sortColumnName);
        });
    },

    create: function () {
        $("#popbox form #Province").off().on("change", function (e) {
            e.preventDefault();
            let provinceCode = $(this).val();

            // Begin submit
            X.A.xhr('/Employee/GetDistrict', !0,
                {
                    provinceCode: provinceCode
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload
                    if (code == 1) {
                        let listDistrict = JSON.parse(message)[0],
                            option = '<option value="">--- Select District ---</option>',
                            optionWard = '<option value="">--- Select Ward ---</option>';

                        if (X.F.is(listDistrict, "undefined")) {
                            $("#District").prop("disabled", !0);
                            $("#Ward").prop("disabled", !0);
                            $("#District").find('option').remove().end().append(option);
                            $("#Ward").find('option').remove().end().append(optionWard);
                            return !1;
                        }

                        let districts = [];
                        $.each(listDistrict, function (i, item) {
                            option += `<option value="${item.Id}">${item.Name}</option>`;
                            districts.push(item);
                        });

                        $("#District").prop("disabled", !1);
                        $("#District").find('option').remove().end().append(option);
                        $("#Ward").find('option').remove().end().append(optionWard);

                        $("#District").off().on('change', function (e) {
                            e.preventDefault();
                            let id = $(this).val();
                            if (id === '' || X.F.is(id, "undefined")) {
                                $("#Ward").prop("disabled", !0);
                                $("#Ward").find('option').remove().end().append(optionWard);
                                return !1;
                            }

                            $.each(districts, function (i, v) {
                                if (v.Id === id) {
                                    let wards = v.Wards;
                                    $.each(wards, function (i, item) {
                                        optionWard += `<option value="${item.Id}">${item.Name}</option>`;
                                    });

                                    $("#Ward").prop("disabled", !1);
                                    $("#Ward").find('option').remove().end().append(optionWard);
                                    return !1;
                                }
                            });
                        });
                    }
                    else {
                        X.F.setError(message);
                    }
                })
            return !1;
        });

        $("#popbox form .uploadFile").off().on("change", function (e) {
            e.preventDefault();
            let input = this;
            if (input.files && input.files[0]) {

                var fileExtension = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];

                if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
                    X.F.setError("Only formats are allowed : " + fileExtension.join(', '));
                    return !1;
                }

                let reader = new FileReader();
                reader.onload = function (e) {
                    $('#popbox form .imagePreview').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
            return !1;
        });

        $("#popbox form").off().on("submit", function (e) {
            e.preventDefault();

            let frm = $(this)
                , token = $("[name=__RequestVerificationToken]", frm).val()
                , firstName = $.trim($("[name=FirstName]", frm).val())
                , lastName = $.trim($("[name=LastName]", frm).val())
                , avatar = $.trim($("[name=Avater]", frm).attr('src'))
                , code = $.trim($("[name=IdentityCartNumber]", frm).val())
                , birthday = $("[name=Birthday]", frm).val()
                , email = $.trim($("[name=Email]", frm).val())
                , gender = $("[name=Gender]", frm).val()
                , phone = $.trim($("[name=Phone]", frm).val())
                , district = $.trim($("[name=District] option:selected", frm).val())
                , province = $.trim($("[name=Province] option:selected", frm).val())
                , ward = $.trim($("[name=Ward] option:selected", frm).val())
                , address = $.trim($("[name=Street]", frm).val())
                , btnSbm = $("[type=submit]", frm);

            if (firstName === "" || lastName === "" || email === "" || token === "") {
                X.F.setError("Type First name + Last name + Email", !1);
                return !1;
            }

            if (code === '') {
                X.F.setError("Type the identity code", !1);
                return !1;
            }

            if (!X.F.isEmail(email)) {
                X.F.setError("Invalid email address", !1);
                return !1;
            }

            if (district === '' || province === '' || ward === '' || address === '') {
                X.F.setError("Type the province + district + ward + address!", !1);
                return !1;
            }

            X.A.xhr(frm.prop("action"), !0,
                {
                    FirstName: firstName,
                    LastName: lastName,
                    Avatar: avatar,
                    IdentityCartNumber: code,
                    Gender: gender,
                    Email: email,
                    Phone: phone,
                    Birthday: birthday,
                    Province: province,
                    District: district,
                    Ward: ward,
                    Address: address,
                    __RequestVerificationToken: token
                }, function () {
                    X.F.setError("", !0);
                    btnSbm.prop("disabled", !0).addClass("disabled");
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload

                    if (code == 1) {
                        X.Thikbox.remove();
                        X.Page.getAjaxList(window.location.href, function () { X.F.doEffect($("#" + message.split("#").pop())) })
                    }
                    else {
                        X.F.setError(message, !1);
                        btnSbm.prop("disabled", !1).removeClass("disabled");
                    }
                })
            return !1;
        });
    },

    edit: function () {

        $("#popbox form #Province").off().on("change", function (e) {
            e.preventDefault();
            let provinceCode = $(this).val();

            // Begin submit
            X.A.xhr('/Employee/GetDistrict', !0,
                {
                    provinceCode: provinceCode
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload
                    if (code == 1) {
                        let listDistrict = JSON.parse(message)[0],
                            option = '<option value="">--- Select District ---</option>',
                            optionWard = '<option value="">--- Select Ward ---</option>';

                        if (X.F.is(listDistrict, "undefined")) {
                            $("#District").prop("disabled", !0);
                            $("#Ward").prop("disabled", !0);
                            $("#District").find('option').remove().end().append(option);
                            $("#Ward").find('option').remove().end().append(optionWard);
                            return !1;
                        }

                        let districts = [];
                        $.each(listDistrict, function (i, item) {
                            option += `<option value="${item.Id}">${item.Name}</option>`;
                            districts.push(item);
                        });

                        $("#District").prop("disabled", !1);
                        $("#District").find('option').remove().end().append(option);

                        $("#District").off().on('change', function (e) {
                            e.preventDefault();
                            let id = $(this).val();
                            if (id === '' || X.F.is(id, "undefined")) {
                                $("#Ward").prop("disabled", !0);
                                $("#Ward").find('option').remove().end().append(optionWard);
                                return !1;
                            }

                            $.each(districts, function (i, v) {
                                if (v.Id === id) {
                                    let wards = v.Wards;
                                    $.each(wards, function (i, item) {
                                        optionWard += `<option value="${item.Id}">${item.Name}</option>`;
                                    });

                                    $("#Ward").prop("disabled", !1);
                                    $("#Ward").find('option').remove().end().append(optionWard);
                                    return !1;
                                }
                            });
                        });
                    }
                    else {
                        X.F.setError(message);
                    }
                })
            return !1;
        });

        $("#popbox form .uploadFile").off().on("change", function (e) {
            e.preventDefault();
            let input = this;
            if (input.files && input.files[0]) {

                var fileExtension = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];

                if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
                    X.F.setError("Only formats are allowed : " + fileExtension.join(', '));
                    return !1;
                }

                let reader = new FileReader();
                reader.onload = function (e) {
                    $('#popbox form .imagePreview').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
            return !1;
        });

        $("#popbox form").off().on("submit", function (e) {
            e.preventDefault();

            let frm = $(this)
                , token = $("[name=__RequestVerificationToken]", frm).val()
                , id = parseInt($("[name=Id]", frm).val())
                , firstName = $.trim($("[name=FirstName]", frm).val())
                , lastName = $.trim($("[name=LastName]", frm).val())
                , avatar = $.trim($("[name=Avater]", frm).attr('src'))
                , code = $.trim($("[name=IdentityCartNumber]", frm).val())
                , birthday = $("[name=Birthday]", frm).val()
                , email = $.trim($("[name=Email]", frm).val())
                , gender = $("[name=Gender]", frm).val()
                , phone = $.trim($("[name=Phone]", frm).val())
                , district = $.trim($("[name=District] option:selected", frm).val())
                , province = $.trim($("[name=Province] option:selected", frm).val())
                , ward = $.trim($("[name=Ward] option:selected", frm).val())
                , address = $.trim($("[name=Address]", frm).val())
                , btnSbm = $("[type=submit]", frm);

            if (firstName === "" || lastName === "" || email === "" || token === "") {
                X.F.setError("Type First name + Last name + Email", !1);
                return !1;
            }

            if (code === '') {
                X.F.setError("Type the identity code", !1);
                return !1;
            }

            if (!X.F.isEmail(email)) {
                X.F.setError("Invalid email address", !1);
                return !1;
            }

            if (district === '' || province === '' || ward === '' || address === '') {
                X.F.setError("Type the province + district + ward + address!", !1);
                return !1;
            }

            X.A.xhr(frm.prop("action"), !0,
                {
                    Id: id,
                    FirstName: firstName,
                    LastName: lastName,
                    Avatar: avatar,
                    IdentityCartNumber: code,
                    Gender: gender,
                    Email: email,
                    Phone: phone,
                    Birthday: birthday,
                    Province: province,
                    District: district,
                    Ward: ward,
                    Address: address,
                    __RequestVerificationToken: token
                }, function () {
                    X.F.setError("", !0);
                    btnSbm.prop("disabled", !0).addClass("disabled");
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload

                    if (code == 1) {
                        X.Thikbox.remove();
                        X.Page.getAjaxList(window.location.href, function () { X.F.doEffect($("#" + message.split("#").pop())) })
                    }
                    else {
                        X.F.setError(message, !1);
                        btnSbm.prop("disabled", !1).removeClass("disabled");
                    }
                })
            return !1;
        });
    },

    init: function () {
        let that = this;

        /*--- 1. Search buttons */
        $("#employee_search_form").off().on("submit", function () {
            that.search_employee("#gridcontainer");
        });

        /*--- 2. Create buttons */
        $("#create").off().on("click", function (e) {
            e.preventDefault();

            // Get ajax create form
            X.A.xhr("/employee/create")
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, "Create", function () {
                        that.create();
                    });
                });
        });

        /*--- 3. Disable + Enable buttons */
        $("[data-action=disable], [data-action=enable]").off().on("click", function (e) {
            e.preventDefault();

            // Load confirm popbox
            let id = parseInt($(this).data("id")),
                action = $(this).data("action");
            if (isNaN(id) || action == "") {
                alert("Invalid action id");
                return !1;
            }

            let title = action === "disable" ? "Disable" : "Enable";

            X.A.xhr("/employee/" + action + "/" + id)
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, title, function () {
                        X.Page.setEnableDisableConfirm()
                    });

                });
        });

        $("[data-action=edit]").off().on("click", function (e) {
            e.preventDefault();
            // Load confirm popbox
            let id = parseInt($(this).data("id")),
                action = $(this).data("action");
            if (isNaN(id) || action == "") {
                alert("Invalid action id");
                return !1;
            }

            let title = "Edit";

            X.A.xhr("/employee/" + action + "/" + id)
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, title, function () {
                        X.Page.Employee.edit()
                    });

                });
        });
    },
}

X.Page.Customer = {
    search_customer: function (jGridContainerSelector, sortColumnName) {
        let frm = $("#customer_search_form")
            , btnSbm = $("[type=submit]", frm)
            , query = $.trim($("[name=query]", frm).val())
            , status = $.trim($("[name=status]", frm).val())
            , urlQuery = location.pathname + "?"
            , urlParams = {};

        (query !== "") ? urlParams.query = X.F.doKeywordFilter(query) : "";
        (parseInt(status) >= 0) ? urlParams.status = status : "";
        (sortColumnName !== "") ? urlParams.sortColumnName = sortColumnName : "";

        urlQuery += $.param(urlParams);
        $("[name=query]", frm).val(urlParams.query);

        X.A.xhr(urlQuery, !1, "",
            function () {
                btnSbm.prop("disabled", true);
            })
            .done(function (data, textStatus, xhr) {
                // Check session               
                X.F.checkSession(xhr);

                // Success return HTML grid
                jGridContainerSelector = jGridContainerSelector || "#gridcontainer";
                $(jGridContainerSelector).html(data);
                X.F.isHistory() ? history.replaceState("", "", urlQuery) : "";
                btnSbm.prop("disabled", false);
            });

        return !1;
    },

    sort_customer: function () {
        $("[data-action=sort]").off().on("click", function (e) {
            e.preventDefault();
            let sortColumnName = $(this).data("field");

            X.Page.Customer.search_customer("#gridcontainer", sortColumnName);
        });
    },

    create: function () {
        $("#popbox form #Province").off().on("change", function (e) {
            e.preventDefault();
            let provinceCode = $(this).val();

            // Begin submit
            X.A.xhr('/Employee/GetDistrict', !0,
                {
                    provinceCode: provinceCode
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload
                    if (code == 1) {
                        let listDistrict = JSON.parse(message)[0],
                            option = '<option value="">--- Select District ---</option>',
                            optionWard = '<option value="">--- Select Ward ---</option>';

                        if (X.F.is(listDistrict, "undefined")) {
                            $("#District").prop("disabled", !0);
                            $("#Ward").prop("disabled", !0);
                            $("#District").find('option').remove().end().append(option);
                            $("#Ward").find('option').remove().end().append(optionWard);
                            return !1;
                        }

                        let districts = [];
                        $.each(listDistrict, function (i, item) {
                            option += `<option value="${item.Id}">${item.Name}</option>`;
                            districts.push(item);
                        });

                        $("#District").prop("disabled", !1);
                        $("#District").find('option').remove().end().append(option);
                        $("#Ward").find('option').remove().end().append(optionWard);

                        $("#District").off().on('change', function (e) {
                            e.preventDefault();
                            let id = $(this).val();
                            if (id === '' || X.F.is(id, "undefined")) {
                                $("#Ward").prop("disabled", !0);
                                $("#Ward").find('option').remove().end().append(optionWard);
                                return !1;
                            }

                            $.each(districts, function (i, v) {
                                if (v.Id === id) {
                                    let wards = v.Wards;
                                    $.each(wards, function (i, item) {
                                        optionWard += `<option value="${item.Id}">${item.Name}</option>`;
                                    });

                                    $("#Ward").prop("disabled", !1);
                                    $("#Ward").find('option').remove().end().append(optionWard);
                                    return !1;
                                }
                            });
                        });
                    }
                    else {
                        X.F.setError(message);
                    }
                })
            return !1;
        });

        $("#popbox form").off().on("submit", function (e) {
            e.preventDefault();

            let frm = $(this)
                , token = $("[name=__RequestVerificationToken]", frm).val()
                , firstName = $.trim($("[name=FirstName]", frm).val())
                , lastName = $.trim($("[name=LastName]", frm).val())
                , code = $.trim($("[name=IdentityCartNumber]", frm).val())
                , birthday = $("[name=Birthday]", frm).val()
                , email = $.trim($("[name=Email]", frm).val())
                , gender = $("[name=Gender]", frm).val()
                , phone = $.trim($("[name=Phone]", frm).val())
                , district = $.trim($("[name=District] option:selected", frm).val())
                , province = $.trim($("[name=Province] option:selected", frm).val())
                , ward = $.trim($("[name=Ward] option:selected", frm).val())
                , address = $.trim($("[name=Street]", frm).val())
                , btnSbm = $("[type=submit]", frm);

            if (firstName === "" || lastName === "" || email === "" || token === "") {
                X.F.setError("Type First name + Last name + Email", !1);
                return !1;
            }

            if (code === '') {
                X.F.setError("Type the identity code", !1);
                return !1;
            }

            if (!X.F.isEmail(email)) {
                X.F.setError("Invalid email address", !1);
                return !1;
            }

            if (district === '' || province === '' || ward === '' || address === '') {
                X.F.setError("Type the province + district + ward + address!", !1);
                return !1;
            }

            X.A.xhr(frm.prop("action"), !0,
                {
                    FirstName: firstName,
                    LastName: lastName,
                    IdentityCartNumber: code,
                    Gender: gender,
                    Email: email,
                    Phone: phone,
                    Birthday: birthday,
                    Province: province,
                    District: district,
                    Ward: ward,
                    Address: address,
                    __RequestVerificationToken: token
                }, function () {
                    X.F.setError("", !0);
                    btnSbm.prop("disabled", !0).addClass("disabled");
                })
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);

                    var data = JSON.parse(data),
                        message = data.message,
                        code = parseInt(data.code);

                    // Success - message will be URL to reload

                    if (code == 1) {
                        X.Thikbox.remove();
                        X.Page.getAjaxList(window.location.href, function () { X.F.doEffect($("#" + message.split("#").pop())) })
                    }
                    else {
                        X.F.setError(message, !1);
                        btnSbm.prop("disabled", !1).removeClass("disabled");
                    }
                })
            return !1;
        });
    },

    init: function () {
        let that = this;
        /*--- 1. Create buttons */
        $("#create").off().on("click", function (e) {
            e.preventDefault();

            // Get ajax create form
            X.A.xhr("/customer/create")
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, "Create", function () {
                        that.create();
                    });
                });
        });

        /*--- 2. Search buttons */
        $("#customer_search_form").off().on("submit", function () {
            that.search_customer("#gridcontainer");
        });

        /*--- 3. Disable + Enable buttons */
        $("[data-action=disable], [data-action=enable]").off().on("click", function (e) {
            e.preventDefault();

            // Load confirm popbox
            let id = parseInt($(this).data("id")),
                action = $(this).data("action");
            if (isNaN(id) || action == "") {
                alert("Invalid action id");
                return !1;
            }

            let title = action === "disable" ? "Disable" : "Enable";

            X.A.xhr("/customer/" + action + "/" + id)
                .done(function (data, textStatus, xhr) {
                    // Check session
                    X.F.checkSession(xhr);
                    X.Thikbox.load(data, title, function () {
                        X.Page.setEnableDisableConfirm()
                    });

                });
        });
    },
}