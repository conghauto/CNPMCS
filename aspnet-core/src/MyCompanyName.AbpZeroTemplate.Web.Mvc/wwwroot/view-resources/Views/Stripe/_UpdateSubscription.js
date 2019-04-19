(function () {

    function prepareStripeButton() {
        var handler = StripeCheckout.configure({
            key: $('#stripePublishableKey').val(),
            locale: 'auto',
            token: function (token) {
                abp.ui.setBusy();
                $('input[name=stripeToken]').val(token.id);
                console.log(token.id);
                $('#stripeCheckoutForm').submit();
            }
        });

        document.getElementById('stripe-checkout').addEventListener('click', function (e) {
            var price = $('input[name=amount]').val();
            handler.open({
                name: 'AbpZeroTemplate',
                description: $('input[name=description]').val(),
                amount: price.replace('.', '')
            });
            e.preventDefault();
        });

        window.addEventListener('popstate', function () {
            handler.close();
        });
    }

    prepareStripeButton();

})();