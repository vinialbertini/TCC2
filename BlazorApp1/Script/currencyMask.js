window.currencyMask = {
    apply: function (element) {
        var formatter = new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        });

        element.addEventListener('input', function (e) {
            var value = e.target.value.replace(/\D/g, '');
            e.target.value = formatter.format(value / 100);
        });
    }
};
