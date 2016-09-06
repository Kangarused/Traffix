var login = {
    username: {
        'validations': 'required'
    },
    password: {
        'validations': 'required'
    }
}

var userAccount = {
   username: {
        'validations': 'required'
    },
   name: {
       'validations': 'required'
   },
   email: {
       'validations': 'required, email'
   },
   password: {
       'validations': 'required',
       'validate-on': 'blur'
   },
   confirmPassword: {
       'validations': 'required, matchPassword',
       'validate-on': 'blur'
   }
}

var addBudget = {
    budgetType: {
        'validations' : 'required'
    },
    name: {
        'validations' : 'required'
    },
    allocatedAmount: {
        'validations' : 'required'
    }
}

angularApplication.config(['validationSchemaProvider', validationSchemaProvider => {
    validationSchemaProvider.set("UserAccount", userAccount);
    validationSchemaProvider.set("Login", login);
    validationSchemaProvider.set("AddBudget", addBudget);
}]);
