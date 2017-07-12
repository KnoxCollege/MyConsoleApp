using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using Jenzabar.JX.Common.WebServices.DataTransferObjects;
using Jenzabar.JX.Common.WebServices.Interfaces;
using Jenzabar.JX.Core.Exceptions;
using Jenzabar.JX.Core.Interfaces;
using System.Linq;


namespace MyConsoleApp
{
    public class AttributeLoader
    {
        private readonly ILogger _logger;
        private readonly IMail _email;
        private readonly ISetup _setup;
        private readonly ITranslator _translator;
        private readonly IAttributeService _attributeService;


        public AttributeLoader(ILogger logger, IMail email, ISetup setup, ITranslator translator, IAttributeService attributeService)
        {
            _logger = logger;
            _email = email;
            _setup = setup;
            _translator = translator;
            _attributeService = attributeService;
        }

        public void Run()
        {
            // Get the location of the file we are going to import from the configuration settings
            string fileLocation = _setup.GetSetting("InvolvementFileLocation");
             //test
            // Initialize the FileHelper engine
            FileHelperEngine<FileFormat> engine = new FileHelperEngine<FileFormat>();

            FileFormat[] result = engine.ReadFile(fileLocation);

            _logger.Log.Info($"{fileLocation} has been loaded.");

            List<AttributeDTO> attributeList = new List<AttributeDTO>();

            foreach (FileFormat row in result)
            {
                AttributeDTO attributeDto = new AttributeDTO(
                    row.ConstituentId,
                    _translator.Translate("AttributeType", row.InvolvementCode))    // Using the ITranslator to convert CX involvement code to attribute code
                {
                    AttributeDoc = row.Comment,
                    BeginsOn = row.BeginsOn,
                    EndsOn = row.EndsOn
                };

                attributeList.Add(attributeDto);
            }

            if (_translator.Warnings.Any())
            {
                string msg =
                    "Translation errors occured in the lookup file.  Please review the log file and correct before posting data.";

                _logger.Log.Error(msg);

                // Send the error message to an email address.  Useful for when this is an automated process.
                //_email.Send(
                //    _setup.GetSetting("SmtpTo"),
                //    _setup.GetSetting("SmtpFrom"),
                //    "JX.Net Example Attribute Loader Error",
                //    msg
                //    );
            }
            else
            {
                // Translation is valid, so lets attempt to create the attributes by interating through the list
                foreach (AttributeDTO attribute in attributeList)
                {
                    IList<AttributeDTO> userAttributes = _attributeService.GetAllByConstituent(attribute.ConstituentId);
                    if (!userAttributes.ToList().Exists(x => x.AttributeType == attribute.AttributeType && x.BeginsOn == attribute.BeginsOn && x.EndsOn == attribute.EndsOn))
                    {
                        try
                        {
                            // Use the Create method of the AttributeService to post the data to JX.
                            AttributeDTO ret = _attributeService.Create(attribute);

                            // The created object is returned from the Create method.  here we can check if its null or not.
                            // If it is null then there was a problem creating the Attribute.
                            if (ret != null)
                                _logger.Log.Info($"Successfully Created {ret.AttributeType} Attribute for {ret.ConstituentId} ");
                            else
                                _logger.Log.Error("Failed to create attribute.  Please review logs.");
                        }
                        catch (MissingDataException ex)
                        {
                            // The MissingDataException is thrown if there are any failed validations that generated an error message and we can assume
                            // that there was insufficient data provided to result in a valid posting of data to JX.
                            _logger.Log.Error(ex);
                        }
                    }
                    else {
                        _logger.Log.Info("Caleb error");
                    }
                }
            }
        }
    }
}
