$(function() {
  'use strict';  
  $('#tabs').tabs({
    activate: function( event, ui ) {        
      renderForm(parseInt(ui.newTab.index())+1);
    }
  });   
    var formBuilder = $('.form-builder-template'); 
  //render the builder just once to initialize it
    formBuilder.formBuilder();

  //render the first tab
  renderForm(1);
    
  // render each tab as we activate it
  function renderForm(id) {      
    var fbTemplate = document.getElementById('fb-template-' + id)
        , formContainer = document.getElementById('form-render-' + id)
        , formRenderOpts = {
          container: formContainer,
            //formData: fbTemplate.value,
            //formData : '[{"type":"text","label":"Full Name","subtype":"text","className":"form-control","name":"text-1476748004559"},{"type":"select","label":"Occupation","className":"form-control","name":"select-1476748006618","values":[{"label":"Street Sweeper","value":"option-1","selected":true},{"label":"Moth Man","value":"option-2"},{"label":"Chemist","value":"option-3"}]},{"type":"textarea","label":"Short Bio","rows":"5","className":"form-control","name":"textarea-1476748007461"}]'
        };
    
    //render the container with options
    $(formContainer).formRender(formRenderOpts);
  }



    var formData = '[{"type":"text","label":"Full Name","subtype":"text","className":"form-control","name":"text-1476748004559"},{"type":"select","label":"Occupation","className":"form-control","name":"select-1476748006618","values":[{"label":"Street Sweeper","value":"option-1","selected":true},{"label":"Moth Man","value":"option-2"},{"label":"Chemist","value":"option-3"}]},{"type":"textarea","label":"Short Bio","rows":"5","className":"form-control","name":"textarea-1476748007461"}]';
    formBuilder.actions.setData(formData);










});