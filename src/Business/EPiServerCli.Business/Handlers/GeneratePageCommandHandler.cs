﻿using EPiServerCli.Business.Interfaces.Handlers;
using EPiServerCli.Business.Interfaces.Static;
using EPiServerCli.DataAccess.Interfaces.Repositories;
using EPiServerCli.Domain.Entities;

namespace EPiServerCli.Business.Handlers
{
    public class GeneratePageCommandHandler : ICommandHandler
    {
        private readonly ITemplateRepository repository;

        public GeneratePageCommandHandler(ITemplateRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(Command command)
        {
            Task<string>? classTemplateTask = repository.GetAsync(Templates.Pages.Class);
            Task<string>? controllerTemplateTask = repository.GetAsync(Templates.Pages.Controller);
            Task<string>? viewTemplateTask = repository.GetAsync(Templates.Pages.View);

            await Task.WhenAll(classTemplateTask, controllerTemplateTask, viewTemplateTask);

            string classTemplate = classTemplateTask.Result;
            string controllerTemplate = controllerTemplateTask.Result;
            string viewTemplate = viewTemplateTask.Result;

            string guid = Guid.NewGuid().ToString();
            string name = command.Name;

            string directoryPath = Directory.GetCurrentDirectory();
            string directoryName = new DirectoryInfo(directoryPath).Name;

            classTemplate = string.Format(classTemplate, guid, name, directoryName);
            controllerTemplate = string.Format(controllerTemplate, name, directoryName);
            viewTemplate = string.Format(viewTemplate, name, directoryName);

            var createClassTask = repository.CreateAsync($"{directoryPath}/Models/Pages/{name}.cs", classTemplate);
            var createControllerTask = repository.CreateAsync($"{directoryPath}/Controllers/{name}.cs", controllerTemplate);
            var createViewTask = repository.CreateAsync($"{directoryPath}/Views/{name}/Index.cshtml", viewTemplate);

            await Task.WhenAll(createClassTask, createControllerTask, createViewTask);
        }
    }
}
