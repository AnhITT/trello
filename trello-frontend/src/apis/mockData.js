export const mockData = {
  board: {
    id: 'board-id-01',
    title: 'Board Name Lam Anh',
    description: 'Pro MERN stack Course',
    type: 'public', // 'private'
    ownerIds: [], // Những users là Admin của board
    memberIds: [], // Những users là member bình thường của board
    workflowOrderIds: ['workflow-id-03', 'workflow-id-02', 'workflow-id-01'], // Thứ tự sắp xếp / vị trí của các workflows trong 1 boards
    workflows: [
      {
        id: 'workflow-id-01',
        boardId: 'board-id-01',
        title: 'To Do workflow 01',
        cardOrderIds: [
          'card-id-01',
          'card-id-02',
          'card-id-03',
          'card-id-04',
          'card-id-05',
          'card-id-06',
          'card-id-07'
        ],
        cards: [
          {
            id: 'card-id-01',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 01',
            description: 'Markdown Syntax (sẽ ở khóa nâng cao nhé)',
            cover:
              'https://trungquandev.com/wp-content/uploads/2022/07/fair-mern-stack-advanced-banner-trungquandev.jpg',
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: ['test comment 01', 'test comment 02'],
            attachments: [
              'test attachment 01',
              'test attachment 02',
              'test attachment 03'
            ]
          },
          {
            id: 'card-id-02',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 02',
            description: null,
            cover: null,
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-03',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 03',
            description: null,
            cover: null,
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-04',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 04',
            description: null,
            cover: null,
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-05',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 05',
            description: null,
            cover: null,
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-06',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 06',
            description: null,
            cover: null,
            memberIds: [
              'test-user-id-01',
              'test-user-id-01',
              'test-user-id-01'
            ],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-07',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-01',
            title: 'Title of card 07',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          }
        ]
      },
      {
        id: 'workflow-id-02',
        boardId: 'board-id-01',
        title: 'Inprogress workflow 02',
        cardOrderIds: ['card-id-08', 'card-id-09', 'card-id-10'],
        cards: [
          {
            id: 'card-id-08',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-02',
            title: 'Title of card 08',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-09',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-02',
            title: 'Title of card 09',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-10',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-02',
            title: 'Title of card 10',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          }
        ]
      },
      {
        id: 'workflow-id-03',
        boardId: 'board-id-01',
        title: 'Done workflow 03',
        cardOrderIds: ['card-id-11', 'card-id-12', 'card-id-13'],
        cards: [
          {
            id: 'card-id-11',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-03',
            title: 'Title of card 11',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-12',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-03',
            title: 'Title of card 12',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          },
          {
            id: 'card-id-13',
            boardId: 'board-id-01',
            workflowId: 'workflow-id-03',
            title: 'Title of card 13',
            description: null,
            cover: null,
            memberIds: [],
            comments: [],
            attachments: []
          }
        ]
      }
    ]
  }
}
