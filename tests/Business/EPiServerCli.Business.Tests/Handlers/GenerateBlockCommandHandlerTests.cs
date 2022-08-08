﻿using EPiServerCli.Business.Handlers;
using EPiServerCli.DataAccess.Interfaces.Repositories;
using EPiServerCli.Domain.Entities;
using EPiServerCli.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EPiServerCli.Business.Tests.Handlers
{
    public class GenerateBlockCommandHandlerTests
    {
        [Fact]
        public void ExecuteAsync_FileInvalidPath_ShouldThrow()
        {
            Command command = GetCommand();

            var templateRepositoryMock = new Mock<ITemplateRepository>();
            templateRepositoryMock!
                .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<NotSupportedException>();

            var commandHandler = new GenerateBlockCommandHandler(templateRepositoryMock.Object);

            Assert.ThrowsAsync<ScaffoldingException>(async () => await commandHandler.ExecuteAsync(command));
        }

        [Fact]
        public void ExecuteAsync_TemplateInvalidFormat_ShouldThrow()
        {
            Command command = GetCommand();

            var templateRepositoryMock = new Mock<ITemplateRepository>();
            templateRepositoryMock!
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Throws<NotSupportedException>();

            var commandHandler = new GenerateBlockCommandHandler(templateRepositoryMock.Object);

            Assert.ThrowsAsync<TemplateFormattingException>(async () => await commandHandler.ExecuteAsync(It.IsAny<Command>()));
        }

        [Fact]
        public void ExecuteAsync_TemplateDoesNotExist_ShouldThrow()
        {
            Command command = GetCommand();

            var templateRepositoryMock = new Mock<ITemplateRepository>();
            templateRepositoryMock!
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Throws<UnauthorizedAccessException>();

            var commandHandler = new GenerateBlockCommandHandler(templateRepositoryMock.Object);

            Assert.ThrowsAsync<TemplateReadingException>(async () => await commandHandler.ExecuteAsync(It.IsAny<Command>()));
        }

        private static Command GetCommand()
        {
            return new Command(
                            CommandType.Generate,
                            ObjectType.Block, 
                            "TestBlock",
                            It.IsAny<IEnumerable<string>>(),
                            It.IsAny<IEnumerable<string>>());
        }
    }
}