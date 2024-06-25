using BusinessLogic_Layer.Service;
using DataAccess_Layer.Interfaces;

namespace BusinessLogic_Layer.Common
{
    public class DeleteChild
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ElasticsearchService _elasticsearchService;

        public DeleteChild(IUnitOfWork unitOfWork, ElasticsearchService elasticsearchService)
        {
            _unitOfWork = unitOfWork;
            _elasticsearchService = elasticsearchService;
        }

        public async Task DeleteChildWorkspace(Guid idWorkspace)
        {
            try
            {
                #region Get board in workspace
                var boardInWorkspace = _unitOfWork.BoardRepository.Find(n => n.WorkspaceId == idWorkspace).ToList();
                var boardIds = boardInWorkspace.Select(t => t.Id).ToList();
                if (boardInWorkspace.Count == 0)
                    return;

                foreach (var item in boardInWorkspace)
                {
                    await DeleteChildBoard(item.Id);
                }
                _unitOfWork.BoardRepository.RemoveRange(boardInWorkspace);
                #endregion

                #region Delete Document Elasticsearch
                var boardDeleteResponse = await _elasticsearchService.DeleteDocumentRangeAsync(boardIds, "workspaces");
                return;
                #endregion
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task DeleteChildBoard(Guid idBoard)
        {
            try
            {
                #region Get workflow in board
                var workflowInBoard = _unitOfWork.WorkflowRepository.Find(n => n.BoardId == idBoard).ToList();
                var workflowIds = workflowInBoard.Select(t => t.Id).ToList();
                if (workflowInBoard.Count == 0)
                    return;

                foreach (var item in workflowInBoard)
                {
                    await DeleteChildWorkflow(item.Id);
                }
                _unitOfWork.WorkflowRepository.RemoveRange(workflowInBoard);
                #endregion

                #region Delete Document Elasticsearch
                var workflowDeleteResponse = await _elasticsearchService.DeleteDocumentRangeAsync(workflowIds, "workspaces");
                return;
                #endregion
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task DeleteChildWorkflow(Guid idWorkflow)
        {
            try
            {
                #region Get task cards in workflow
                var taskCardInWorkflow = _unitOfWork.TaskCardRepository.Find(n => n.WorkflowId == idWorkflow).ToList();
                var taskCardIds = taskCardInWorkflow.Select(t => t.Id).ToList();

                if (taskCardInWorkflow.Count() == 0)
                    return;
                foreach (var item in taskCardInWorkflow)
                {
                    await DeleteChildTaskCard(item.Id);
                }
                _unitOfWork.TaskCardRepository.RemoveRange(taskCardInWorkflow);
                #endregion

                #region Delete Document Elasticsearch
                var taskCardDeleteResponse = await _elasticsearchService.DeleteDocumentRangeAsync(taskCardIds, "workspaces");
                return;
                #endregion
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task DeleteChildTaskCard(Guid idTaskCard)
        {
            try
            {
                #region Get check list in task card
                var checkListChild = _unitOfWork.CheckListRepository.Find(n => n.TaskId == idTaskCard).ToList();
                if (checkListChild.Count() == 0)
                    return;
                foreach (var item in checkListChild)
                {
                    await DeleteChildCheckList(item.Id);
                }
                _unitOfWork.CheckListRepository.RemoveRange(checkListChild);
                #endregion

                #region Get comment in task card
                var commentChild = _unitOfWork.CommentRepository.Find(n => n.TaskId == idTaskCard).ToList();
                _unitOfWork.CommentRepository.RemoveRange(commentChild);
                #endregion

                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task DeleteChildCheckList(Guid idCheckList)
        {
            try
            {
                var checkListItem = _unitOfWork.CheckListItemRepository.Find(n => n.CheckListId == idCheckList).ToList();
                if (checkListItem.Count() == 0)
                    return;
                _unitOfWork.CheckListItemRepository.RemoveRange(checkListItem);

                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task DeleteChildComment(Guid idComment)
        {
            try
            {
                var commentChild = _unitOfWork.CommentRepository.Find(n => n.ParentCommentId == idComment).ToList();
                if (commentChild.Count() == 0)
                    return;
                foreach (var child in commentChild)
                {
                    await DeleteChildComment(child.Id); 
                }
                _unitOfWork.CommentRepository.RemoveRange(commentChild);
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
