using Moq;
using ValidadorSenhaSegura.Application.UseCases;
using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Tests.Application.UseCases
{
    public class UseCasePasswordValidateTests
    {
        private readonly Mock<IPasswordValidator> _validatorV1Mock;
        private readonly Mock<IPasswordValidator> _validatorV2Mock;

        public UseCasePasswordValidateTests()
        {
            _validatorV1Mock = new Mock<IPasswordValidator>();
            _validatorV2Mock = new Mock<IPasswordValidator>();

            _validatorV1Mock.Setup(v => v.ApiVersion).Returns(ApiVersion.V1_);
            _validatorV2Mock.Setup(v => v.ApiVersion).Returns(ApiVersion.V2_);
        }

        [Fact]
        public void Execute_ShouldReturnValidResponse_WhenPasswordIsValid()
        {
            // Arrange
            _validatorV1Mock
                .Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(ValidationResult.Success());

            var validators = new List<IPasswordValidator> { _validatorV1Mock.Object };

            var useCase = new UseCasePasswordValidate(validators)
                .SetStrategy(ApiVersion.V1_);

            // Act
            var response = useCase.Execute("Senha123!");

            // Assert
            Assert.Equal("1", response.ApiVersion);
            Assert.Equal("A senha informada é válida", response.Data);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public void Execute_ShouldReturnInvalidResponse_WhenPasswordIsInvalid()
        {
            // Arrange
            var validationResult = ValidationResult.Failure(
            [
                "Erro 1",
                "Erro 2"
            ]);

            _validatorV2Mock
                .Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(validationResult);

            var validators = new List<IPasswordValidator> { _validatorV2Mock.Object };

            var useCase = new UseCasePasswordValidate(validators)
                .SetStrategy(ApiVersion.V2_);

            // Act
            var response = useCase.Execute("abc");

            // Assert
            Assert.Equal("2", response.ApiVersion);
            Assert.Equal("A senha informada é inválida, pois não atende aos critérios", response.Data);
            Assert.Equal(2, response.Errors.Count);
        }

        [Fact]
        public void Execute_ShouldUseCorrectValidator_BasedOnApiVersion()
        {
            // Arrange
            _validatorV1Mock
                .Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(ValidationResult.Success());

            _validatorV2Mock
                .Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(ValidationResult.Failure("erro"));

            var validators = new List<IPasswordValidator>
            {
                _validatorV1Mock.Object,
                _validatorV2Mock.Object
            };

            var useCase = new UseCasePasswordValidate(validators);

            // Act
            var responseV1 = useCase.SetStrategy(ApiVersion.V1_).Execute("Qualquer!");
            var responseV2 = useCase.SetStrategy(ApiVersion.V2_).Execute("Qualquer!");

            // Assert
            Assert.Empty(responseV1.Errors);  // versão 1 retorna sucesso
            Assert.Single(responseV2.Errors); // versão 2 retorna falha

            _validatorV1Mock.Verify(v => v.Validate(It.IsAny<string>()), Times.Once);
            _validatorV2Mock.Verify(v => v.Validate(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Execute_ShouldThrowException_WhenValidatorForVersionDoesNotExist()
        {
            // Arrange: lista vazia
            var validators = new List<IPasswordValidator>();
            var useCase = new UseCasePasswordValidate(validators);

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                useCase.SetStrategy(ApiVersion.V1_);
                useCase.Execute("Senha123!");
            });
        }

        [Fact]
        public void SetStretegy_ShouldStoreApiVersionCorrectly()
        {
            // Arrange
            var validators = new List<IPasswordValidator> { 
                _validatorV1Mock.Object, 
                _validatorV2Mock.Object 
            };
            var useCase = new UseCasePasswordValidate(validators);

            // Act
            var returned = useCase.SetStrategy(ApiVersion.V2_);

            // Assert
            Assert.Same(useCase, returned); // fluent interface
        }
    }
}
