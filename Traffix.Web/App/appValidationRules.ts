(() => {
    angular.module('validation.rule', ['validation'])
        .config(['$validationProvider',
            $validationProvider => {

                var expression = {
                    required(value) {
                        return value != null && (typeof value != "string" || value.trim()!=="");
                    },
                    url: /((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)?/,
                    email: (value, scope) => {
                        if (value == null || value == "")
                            return true;

                        return emailAddresses.parseOneAddress(value) != null;
                    },
                    number: /^\d+$/,
                    matchPassword:(value, scope) => {
                        return scope.vm.newUserAccount.password == value;
                    }
                };

                var defaultMsg = {
                    required: {
                        error: 'This field is required.',
                        success: 'It\'s Required'
                    },
                    url: {
                        error: 'This should be Url',
                        success: 'It\'s Url'
                    },
                    email: {
                        error: 'The format of this email address is not valid.',
                        success: 'It\'s Email'
                    },
                    number: {
                        error: 'Please provide a number.',
                        success: 'It\'s Number'
                    },
                    matchPassword: {
                        error: 'Passwords do not match.',
                        success: 'Passwords do match.'
                    },
                    passwordNeeded: {
                        error: 'Specify password',
                        success: 'Password is specified.'
                    }
                };
                $validationProvider.setExpression(expression).setDefaultMsg(defaultMsg);
            }
        ]);

}).call(this);
