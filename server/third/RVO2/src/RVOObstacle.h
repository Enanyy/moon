#ifndef RVO_RVOOBSTACLE_H_
#define RVO_RVOOBSTACLE_H_

#include <vector>
#include "Vector2.h"
using namespace std;

namespace RVO {
	class RVOObstacle
	{
	public:
		RVOObstacle():obstacles_() {

		}
		~RVOObstacle()
		{
			obstacles_.clear();
		}

		void push_back(Vector2& obstacle)
		{
			obstacles_.push_back(std::move(obstacle));
		}

		void clear()
		{
			obstacles_.clear();
		}

		const std::vector<Vector2>& data()const
		{
			return obstacles_;
		}
		size_t size() const
		{
			return obstacles_.size();
		}

	private:
		std::vector<Vector2> obstacles_;
	};
}
#endif
