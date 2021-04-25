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

                    if (code == 0) {
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
        $(function () {
            $(document).on("change", ".uploadFile", function () {

                var uploadFile = $(this);
                var files = !!this.files ? this.files : [];
                if (!files.length || !window.FileReader) return; // no file selected, or no FileReader support

                if (/^image/.test(files[0].type)) { // only image file
                    var reader = new FileReader(); // instance of the FileReader
                    reader.readAsDataURL(files[0]); // read the local file

                    reader.onloadend = function () { // set image data as background of div                        
                        //alert(uploadFile.closest(".upimage").find('.imagePreview').length);
                        uploadFile.closest(".imgUp").find('.imagePreview').css("background-image", "url(" + this.result + ")");
                    }
                }

            });
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
    },
}