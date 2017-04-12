

function SetPorcentageOnHallOfFame(param1, param2, param3) {
    $(document).ready(function () {

        $("#uservalueHallofFame1").text(param1 + "%");

        $("#usercontentbar1").css({
            background: "#79be21",
            height: "10px",
            width: param1 + "%",
        });

        if (param2 != null) {

            $("#usercontentbar2").css({
                background: "#79be21",
                height: "10px",
                width: param2 + "%",
            });
            $("#uservalueHallofFame2").text(param2 + "%");
        }
        if (param3 != null) {

            $("#usercontentbar3").css({
                background: "#79be21",
                height: "10px",
                width: param3 + "%",
            });
            $("#uservalueHallofFame3").text(param3 + "%");
        }


    });

}