$(document).ready(function () {
    $.ajax(
        {
            type: 'GET',
            url: '/Customer/Order/TrackingOrder',
            success: function (result) {
                $('#trackingOrdering').html( "Tracking Order" + "("+result+")");
            },
            //error: function (req, status, error) {
            //    alert(error);
            //}
        });

    $('#btn-addingtocart').click(function () {
        var userQuatity = $(".param param-inline dd input.form-control").val();

        $.ajax({
            type: 'GET',
            url: '/Customer/Home/Details',
            success: function (result) {
                if (result <= userQuatity)
                {
                    alert();
                }
            }
        });
    });
})


