﻿$(document).ready(function () {
    $('.image-container').slick();
    $('.main-image').slick({ arrows: false, dots: true, autoplay: true });
    refreshCart(false, false);
});
function countCardItem(id, count) {
    $.ajax({
        type: "POST",
        url: "/Cart/CountCartItem",
        data: { itemId: id, delta: count },
        success: function (res) {
            refreshCart(true, false);
        },
        datatype: "json",
        traditional: true
    });
}
function removeCardItem(id) {
    $.ajax({
        type: "POST",
        url: "/Cart/RemoveCartItem",
        data: { itemId: id },
        success: function (res) {
            refreshCart(false, true);
        },
        datatype: "json",
        traditional: true
    });
}
function refreshCart(shouldShowCart, shouldAnimate) {
    $.ajax({
        type: "POST",
        url: "/Cart/GetCartItems",
        success: function (res) {
            if (res) {
                $('.cart-sidebar').empty();
                $('.cart-sidebar').append(res);
                if (shouldShowCart) {
                    if (shouldAnimate)
                        showCart();
                    else
                        showCartImediately();
                }
                else {
                    if (shouldAnimate)
                        hideCart();
                    else
                        hideCartImediately();
                }
            }
        },
        datatype: "json",
        traditional: true
    });
}
function showCartImediately() {
    $('.cart-sidebar').css("right", "0px");
    $('#hide-cart').css("visibility", "visible");
    $('#show-cart').css("visibility", "hidden");
}
function hideCartImediately() {
    $('.cart-sidebar').css("right", "-370px");
    $('#hide-cart').css("visibility", "hidden");
    $('#show-cart').css("visibility", "visible");
}
function showCart() {
    $('.cart-sidebar').css("animation", "0.3s rollin");
    showCartImediately();
}
function hideCart() {
    $('.cart-sidebar').css("animation", "0.3s rollout");
    hideCartImediately();
}
$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() == $(document).height()) {
        $('.footer').css("opacity", 1);
    } else {
        $('.footer').css("opacity", 0);
    }
});

function buyItem(id) {
    $.ajax({
        type: "POST",
        url: "/Cart/AddToCart",
        data: { itemId: id },
        success: function (res) {
            refreshCart(true, true);
        },
        datatype: "json",
        traditional: true
    });
}