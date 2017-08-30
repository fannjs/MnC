<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="specific.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advertisement.specific" %>

<link rel="Stylesheet" href="../../../assets/styles/advertisement.css" />

<script type="text/javascript">
    var num = 0;

    function addIntoSeq() {
        $('#advertisement-maintenance-main-div').on('click', '.fa-plus', function () {
            $(this).closest('.image-wrapper').clone().appendTo('#advertisement-seq');
        });
    }

    function getAdDetailsSuccess(data) {
        alert("specific getAdDetailsSuccess");

        var num;
        var images = "";
        for (var i = 0; i < data.d.length; i++) {
            num++;
            if (data.d[i].ADV_TYPE == "image") {
                images = images + '<div class="image-wrapper image-uploaded"><div class="image-wrapper-header">' + data.d[i].ADV_FILENAME + ' [' + data.d[i].ADV_ID + '] </div><div class="image-wrapper-body"><img class="image-div" src="' + data.d[i].ADV_FILE_SRC + '" title="' + data.d[i].ADV_FILENAME + '" saved="yes" id="' + data.d[i].ADV_ID + '" />'
                            + '</div><div class="image-wrapper-footer"><i class="fa fa-plus" style="color:green;"></i><i class="fa fa-times" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>';
            } else if (data.d[i].ADV_TYPE == "video") {
                images = images + '<div class="image-wrapper image-uploaded"><div class="image-wrapper-header">' + data.d[i].ADV_FILENAME + ' [' + data.d[i].ADV_ID + '] </div>';
                images = images + '<div class="image-wrapper-body"><video controls class="image-div" src="' + data.d[i].ADV_FILE_SRC + '" title="' + data.d[i].ADV_FILENAME + '" saved="yes" id="' + data.d[i].ADV_ID + '" /></div>';
                images = images + '<div class="image-wrapper-footer"><i class="fa fa-plus" style="color:green;"></i><i class="fa fa-times" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>';
            }

        }
        $('#advertisement-uploaded').html(images);
    };
    function getAdDetailsError(data) {
        alert("Error to retrieve advertisement details!");
    };

    //function saveSequence() {
    //    var items = document.getElementById("advertisement-seq").getElementsByClassName("image-wrapper image-uploaded");
    //    var strseq = "";
    //    var arrAdv = new Array();

    //    for (var i = 0; i < items.length; i++) {
    //        var child = items[i].getElementsByClassName("image-div");
    //        for (var k = 0; k < child.length; k++) {
    //            var advid = "";
    //            if (child[k].nodeName == "IMG") {
    //                advid = child[k].id;
    //                arrAdv[i] = advid;
    //            } else if (child[k].nodeName == "VIDEO") {
    //                advid = child[k].id;
    //                arrAdv[i] = advid;
    //            }
    //        }
    //    }
    //    insertSeqToDB(arrAdv);
    //}
    function insertSeqToDB(arrAdv) {

        var para = {
            'arrAdv': arrAdv
        };

        var objurl =
        {
            'url': 'setAdvSeq',
            'data': para
        };

        __JSONWEBSERVICE.getServices(objurl, setAdvSeqSuccess, setAdvSeqError);
    }

    function setAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to saved the sequence due to record exists!");
        } else {
            alert("Failed to saved the sequence due to database error!");
        }
    };

    function setAdvSeqError(msg) {
        alert('Error: Failed to saved the sequence!');
    };


    function getAdDefaultSeqSuccess(data) {

        $('#advertisement-seq').empty();

        if (data.d.length == 0) {
            $('#advertisement-seq').html('<span id="seq-empty-msg" style="color:#999;">Nothing in the sequence.</span><div class="block-xs"></div>');
        }
        else {
            $('#advertisement-seq').append('<ul></ul>');

            for (var i = 0; i < data.d.length; i++) {
                if (data.d[i].ADV_TYPE == "image") {
                    $('#advertisement-seq > ul').append('<li><div class="image-wrapper seq-img" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '" title="' + data.d[i].ADV_FILENAME + '">'
                                + '<div class="image-wrapper-header"><span class="remove-seq-img"><i class="fa fa-minus-circle"></i></span>' + data.d[i].ADV_FILENAME + '</div>'
                                + '<div class="image-wrapper-body"><img class="image-div" src="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" title="' + data.d[i].ADV_FILENAME + '" />'
                                + '</div></div></li>');
                } else if (data.d[i].ADV_TYPE == "video") {
                    var filename = data.d[i].ADV_FILENAME.substr(0, data.d[i].ADV_FILENAME.lastIndexOf('.'));
                    $('#advertisement-seq > ul').append('<li><div class="image-wrapper seq-img" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '" title="' + data.d[i].ADV_FILENAME + '">'
                                + '<div class="image-wrapper-header"><span class="remove-seq-img"><i class="fa fa-minus-circle"></i></span>' + data.d[i].ADV_FILENAME + '</div>'
                                + '<div class="image-wrapper-body"><video controls class="image-div" controls preload="none" poster="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" class="image-div" src="../views/kioskMaintenance/advertisement/Adv/mp4/' + data.d[i].ADV_ID + '.mp4"  title="' + data.d[i].ADV_FILENAME + '" />'
                                + '</div></div></li>');
                }
            }

            $('#advertisement-seq > ul').sortable({
                containment: "#advertisement-seq-div"
            });

            $('#saveSequenceBtn').show();
        }
    };
    function getAdDefaultSeqError(data) {
        alert("Error to retrieve advertisement sequence!");
    };

    function delAdv(advid) {
        alert("go to delete adv" + advid);
    }

    function saveMachineSeq() {
        var machID = document.getElementById("machID").value;
        var items = document.getElementById("advertisement-seq").getElementsByTagName("ul");
        var strseq = "";
        var arrAdv = [];

        $('#advertisement-seq > ul li').each(function () {
            var advSeq = $(this).children('.seq-img').attr('data-advID');

            if (advSeq.length == 1) {
                strseq += "0" + advSeq;
            } else {
                strseq += advSeq;
            }
            arrAdv.push(advSeq);
        });

        if (strseq.length < 30) {
            for (var i = 0; i < 30; i++) {
                strseq = strseq + "0";
                if (strseq.length == 30) {
                    break;
                }
            }
        }

        var listMachid = machID.split(',');
        var arrMachid = new Array();
        var cnt = 0;
        for (var j = 0; j < listMachid.length; j++) {
            if (listMachid[j].trim() != "") {
                arrMachid[cnt] = listMachid[j].trim();
                cnt++;
            }
        }

        //for (var i = 0; i < items.length; i++) {
        //    var child = items[i].getElementsByClassName("image-div");
        //    for (var k = 0; k < child.length; k++) {
        //        var advid = "";

        //        advid = child[k].id;
        //        arrAdv[i] = advid;

        //        if (advid.length == 1) {
        //            strseq += "0" + advid;
        //        } else if (advid.length == 2) {
        //            strseq += advid;
        //        }
        //    }
        //}

        //if (strseq.length < 30) {
        //    for (var i = 0; i < 30; i++) {
        //        strseq = strseq + "0";
        //        if (strseq.length == 30) {
        //            break;
        //        }
        //    }
        //}
        //insertMachSeqToDB(machID, arrAdv, strseq);
        insertMachincesSeqToDB(arrMachid, arrAdv, strseq);
    }

    function insertMachincesSeqToDB(arrMachid, arrAdv, strseq) {
        var para = {
            'arrMachid': arrMachid,
            'arrAdv': arrAdv,
            'machSeq': strseq
        };

        var objurl =
        {
            'url': 'setMachinesAdvSeq',
            'data': para
        };
        __JSONWEBSERVICE.getServices(objurl, setMachinesAdvSeqSuccess, setMachinesAdvSeqError);
    }

    function setMachinesAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Specific machine advertisement sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to save machine sequence due to record exists!");
        } else {
            alert("Failed to save machine sequence due to database error!");
        }
    };

    function setMachinesAdvSeqError(msg) {
        alert('Error: Failed to set machine sequence!');
    };

    function insertMachSeqToDB(machID, arrAdv, strseq) {
        var para = {
            'machID': machID,
            'arrAdv': arrAdv,
            'machSeq': strseq
        };

        var objurl =
        {
            'url': 'setMachAdvSeq',
            'data': para
        };
        __JSONWEBSERVICE.getServices(objurl, setMachAdvSeqSuccess, setMachAdvSeqError);
    }

    function setMachAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Specific machine advertisement sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to save machine sequence due to record exists!");
        } else {
            alert("Failed to save machine sequence due to database error!");
        }
    };

    function setMachAdvSeqError(msg) {
        alert('Error: Failed to set machine sequence!');
    };

    function removeSeqImg() {
        $('#advertisement-maintenance-main-div').on('click', '.remove-seq-img', function (event) {

            $(this).closest('li').remove();

            if ($('#advertisement-seq > ul li').length == 0) {
                $('#advertisement-seq').html('<span id="seq-empty-msg" style="color:#999;">Nothing in the sequence.</span><div class="block-xs"></div>');
            }
        });
    }

    $(document).ready(function () {

        __JSONWEBSERVICE.getServices("getAdDefaultSeq", getAdDefaultSeqSuccess, getAdDefaultSeqError);
        //uploadAdv();
        addIntoSeq();
        removeSeqImg();

        $('#advertisement-seq').sortable({
            containment: "#advertisement-seq"
        });

        $('.image-wrapper').first().remove();
    });
</script>

<div id="advertisement-maintenance-main-div">
    <div id="advertisement-seq-div">
        <h4>Advertisement Media Display Sequence</h4>
        <div id="advertisement-seq">
            <div class="image-wrapper"></div>
        </div>
    <%--<button class="btn btn-primary" onclick="saveSequence()">Save Sequences</button>--%>
    </div>
    <br />
    <div id="Div1">
        <hr />
        <h4>Specific Kiosk setting </h4>
        <div id="Div2">
            <p>Machine ID: </p>
            <input type="text" ID="machID" value="01CD"/>
        </div>
        <br />
    <button class="btn btn-primary" onclick="saveMachineSeq()">Save Sequences</button>
    </div>

</div>
