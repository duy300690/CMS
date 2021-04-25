/*!------------------------
  Xicooc devlib JS
  fb.com/xicooc
  ------------------------*/

/* Global namespace object */
var X = {};
/*-----------------------
  X.Page namspace object
  -----------------------*/
X.Page = {};


X.A =
{
    /*
      Show Thikbox with ajax options
      @param String url to request      
      @param Boolean isPost - default FALSE = GET
      @param Object{} data for POST default null
      @param Function beforeSendFunction callback
      @param Boolean isTraditional serialization - default FALSE
      @return jQuery Ajax Object
     */
    xhr: function (url, isPost, data, beforeSendFunction, isTraditional) {

        isPost = false == isPost || "undefined" == typeof isPost ? false : true;
        return $.ajax({
            url: url,
            type: isPost ? "POST" : "GET",
            data: data,
            traditional: "undefined" == typeof isPost ? false : isTraditional,
            beforeSend: function () {
                // option function                
                "function" == typeof beforeSendFunction ? beforeSendFunction() : "";
            }
        })
            .always(function () {

            })
    }
};

X.F = {
    /**
     is returns a boolean for if typeof obj is exactly type
     */
    is: function (obj, type) {
        return typeof obj === type;
    },

    /**
     * Remove all no-need characters
     */
    doKeywordFilter: function (str) {
        str = str
            .replace(/\s\s+/g, "")
            .replace(/[\/\!\|\\_?*$+#&><=^~"`'%\{\}\(\)]/gi, "");
        return $.trim(str);
    },

    /*
    Check data isJson
    */
    isJSON: function isJson(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return !1;
        }
        return !0;
    },

    /**
      is history pushState API
     */
    isHistory: function () {
        return !!history.pushState;
    },

    /*
      Check session timeout or not
      @param xhr ajax string header [X-Responded-JSON]
     */
    checkSession: function (xhr) {
        var data = xhr.getResponseHeader("X-Responded-JSON"), status, url;
        if (data !== null && X.F.isJSON(data)) {
            data = JSON.parse(data);
            status = parseInt(data.status);
            url = data.headers["location"];

            if ((status == 401 || status == 403) && X.F.isUrl(url)) {
                X.ThikBox.remove();
                alert("Session timed out.\nPlease re-login!\n-----------------------\nPhiên làm việc của bạn đã hết.\nXin đăng nhập lại!");
                location.href = url;
                return !1;
            }
        }
    },

    /*
       Do effect for any jObject
       @param jQuery Object jObj
       @param Function callback
       @param String effecName (default is "bounceIn". Can be "fadeInUp", "pop", "highlight", "bounce")
    */
    doEffect: function (jObj, callback, effectName) {
        var HIGHLIGHT_CLASS = "animated " + ("string" == typeof effectName ? effectName : "bounceIn"),
            OLD_BACKGROUND = jObj.css("background-color")
        jObj
            .removeClass(HIGHLIGHT_CLASS)

            .animate({ backgroundColor: "#FBE983" }, 1).delay(800)
            .addClass(HIGHLIGHT_CLASS)
            .animate({ backgroundColor: OLD_BACKGROUND ? OLD_BACKGROUND : "transparent" }, 300, function () {
                jObj.removeClass(HIGHLIGHT_CLASS);
                if ("function" == typeof callback) {
                    callback();
                }
            })
    },

    setError: function (message, isLoading, jSelector) {        
        var HIGHLIGHT_CLASS = "animated bounce"
            , errorDiv = X.F.is(jSelector, "undefined") ? $(".show_error") : $(jSelector)
            , msg = isLoading ? "Loading" : message
            , exec = function () {
                errorDiv
                    .html(msg)
                    .removeClass(HIGHLIGHT_CLASS)
                    .show(0, function () { $(this).addClass(HIGHLIGHT_CLASS) });

                false == isLoading || X.F.is(isLoading, "undefined")
                    ? errorDiv.removeClass("loading")
                    : errorDiv.addClass("loading");
            };
        // Executive
        msg !== "" ? exec() : errorDiv.hide().empty();
    },

};

X.Page = {};

X.Thikbox = {
    load: function (dataHTML, header, callBack) {
        let modalPupup = "popbox"
            , pupupContent = '';

        if ($("#gridcontainer").find('#' + modalPupup).length > 0) {
            $('#' + modalPupup).remove();
        }

        pupupContent = '<div class="modal fade" id="' + modalPupup + '" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"aria-hidden="true">';
        pupupContent += '       <div class="modal-dialog modal-dialog-centered" role="document">';
        pupupContent += '           <div class="modal-content">';
        pupupContent += '             <div class="modal-header">';
        pupupContent += '               <h4 class="modal-title w-100 font-weight-bold">' + header + '</h4>';
        pupupContent += '               <button type="button" class="close" data-dismiss="modal" aria-label="Close">';
        pupupContent += '                   <span aria-hidden="true">&times;</span>';
        pupupContent += '               </button>';
        pupupContent += '             </div>';

        pupupContent += '             <div class="modal-body mx-3">';
        pupupContent += dataHTML;
        pupupContent += '             </div>';
        pupupContent += '           </div>';
        pupupContent += '       </div>';
        pupupContent += '   </div>';


        $("#gridcontainer").append($(pupupContent));
        $('#' + modalPupup).modal({ show: true, backdrop: 'static', keyboard: false });

        // callback
        "function" == typeof callBack ? callBack() : "";
    },

    /*
     Close Thikbox (remove DOM)
    */
    remove: function () {
        $("#popbox, .modal-backdrop").remove();
    },

}

