(function () {

    function preparePaypalButton() {
        window.paypal.request.addHeaderBuilder(function() {
            return { 'X-XSRF-TOKEN': abp.security.antiForgery.getToken() };
        });

        window.paypal.Button.render({
            style: { size: 'responsive' },
            env: $('input[name=paypal-environment]').val(),
            commit: true,
            client: {
                sandbox: $('input[name=clientId]').val(),
                production: ''
            },
            payment: function (data, actions) {
                return actions.payment.create({
                    transactions: [{
                        amount: {
                            total: $('input[name=Amount]').val(),
                            currency: 'USD'
                        }
                    }]
                });
            },
            onAuthorize: function (data) {
                finishPayment(data);
            },
            onCancel: function (data, actions) {
                window.paypal.request.post(abp.appPath + 'Payment/CancelPayment',
                    {
                        gateway: 'PayPal',
                        paymentId: data.paymentID
                    });
            }
        }, '#paypal-button');
    }

    function finishPayment(data) {
        $('input[name=PayPalPaymentId]').val(data.paymentID);
        $('input[name=PayPalPayerId]').val(data.payerID);

        $('#payPalCheckoutForm').submit();
    }

    preparePaypalButton();

})();